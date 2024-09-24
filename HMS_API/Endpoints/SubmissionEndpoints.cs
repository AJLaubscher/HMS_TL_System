using System;
using HMS_API.Data;
using HMS_API.Dtos.submission;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class SubmissionEndpoints
{
    const string GetSubmissionEndpointName = "GetSubmission";
    public static RouteGroupBuilder MapSubmissionEndpoints(this WebApplication app)
    {
        var submission = app.MapGroup("submissions")
                        .WithParameterValidation();

                // requests for all submissions
            submission.MapGet("/", async (HMS_Context db) => 
            await db.Submissions
            .Include(submission => submission.UserAccount)              // include userAccount entity to get info as in DTO
            .Include(submission => submission.Assignment)               // include assignment entity to get info as in DTO
            .Select(submission => submission.ToSubmissionSummaryDto())
            .AsNoTracking()                                         // optimize query
            .ToListAsync());                                        // asynchronous (async, await ToListAsync())

            // get a specific submission
            submission.MapGet("/{id}", async (int id, HMS_Context db) => {

            if(id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
            }

                Submission? submission = await db.Submissions.FindAsync(id); // find id in db

                return submission is null? Results.NotFound() : Results.Ok(submission.ToSubmissionDetailsDto()); // if null return not found/ return ok if found
            })
            .WithName(GetSubmissionEndpointName);


        // post, create subbmision / add HMS_Context db as parameter for db mapping
            submission.MapPost("/", async (CreateSubmissionDto newSubmission, HMS_Context db) => {

            Submission submission = newSubmission.ToEntity();
            // submission.User = db.Users.Find(submission.Stud_id); // find student id 
            // submission.Assignment = db.Assignments.Find(submission.Assign_id); // find assignment id

            db.Submissions.Add(submission); // add to db
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetSubmissionEndpointName , new {id = submission.Id}, submission.ToSubmissionDetailsDto());
        }).WithParameterValidation();


        // Put = update submission
        submission.MapPut("/{id}", async (int id, UpdateSubmissionDto updatedSubmission, HMS_Context db) =>{

            var existingSubmission = await db.Submissions.FindAsync(id);

            if(existingSubmission == null) // submission does not exist
            {
                return Results.NotFound();
            }

            db.Entry(existingSubmission).CurrentValues.SetValues(updatedSubmission.ToEntity(id)); // set updated values in database
            await db.SaveChangesAsync();

            return Results.NoContent(); // updated
        });


        // Delete submission
        submission.MapDelete("/{id}", async (int id, HMS_Context db) => {

            await db.Submissions.Where(submission => submission.Id == id)      // select submission to remove
                .ExecuteDeleteAsync();                                       // execute

            return Results.NoContent();                                     // deletes or not, does not matter
        });


        return submission;
    }
}
