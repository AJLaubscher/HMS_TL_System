using System;
using HMS_API.Data;
using HMS_API.Dtos.feedback;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class FeedbackEndpoints
{
    const string GetFeedbackEndpointName = "GetFeedback";

    public static RouteGroupBuilder MapFeedbackEndpoints(this WebApplication app)
    {
        var feedback = app.MapGroup("feedback")
                            .WithParameterValidation();

            // requests for feedback
        feedback.MapGet("/", async (HMS_Context db) => 
            await db.Feedbacks
            .Include(feedback => feedback.Submission)                   // include submission entity to get info as in DTO
            .Include(feedback => feedback.UserAccount)                 // include user entity to get info as in DTO
            .Select(feedback => feedback.ToFeedbackSummaryDto()) 
            .AsNoTracking()                                 // optimize qeury 
            .ToListAsync()
        );                               

        //getfeed
        feedback.MapGet("/{id}", async (int id, HMS_Context db) => {
            
            Feedback? feedback = await db.Feedbacks.FindAsync(id);

            // Return NotFound if the assignment is null, otherwise return Ok
            return feedback is null ? Results.NotFound() : Results.Ok(feedback.ToFeedbackDetailsDto());
        }).WithName(GetFeedbackEndpointName);

        // post feedback / add HMS_Context db as parameter for db mapping
        feedback.MapPost("/", async (CreateFeedbackDto newFeedback, HMS_Context db) => {

            Feedback feedback = newFeedback.ToEntity();

            db.Feedbacks.Add(feedback);
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetFeedbackEndpointName, new {id = feedback.Id}, feedback.ToFeedbackDetailsDto());

        }).WithParameterValidation();


        // Put = update assignment
            feedback.MapPut("/{id}", async (int id, UpdateFeedbackDto updateFeedback, HMS_Context db) =>{

                var existingFeedback = await db.Feedbacks.FindAsync(id);

                if(existingFeedback == null) // feedback does not exist
                {
                    return Results.NotFound();
                }

                db.Entry(existingFeedback).CurrentValues.SetValues(updateFeedback.ToEntity(id)); // set updated values in database

                await db.SaveChangesAsync();

                return Results.NoContent(); // updated
            });

            // other code if the above does not work
            // Put = update feedback
            // feedback.MapPut("/{id}", async (int id, UpdateFeedbackDto updateFeedback, HMS_Context db) =>
            // {
            //     var existingFeedback = await db.Feedbacks.FindAsync(id);

            //     if (existingFeedback == null) // feedback does not exist
            //     {
            //         return Results.NotFound();
            //     }

            //     // Explicitly map fields
            //     existingFeedback.Comment = updateFeedback.Comment; // Assuming Comment is a field
            //     existingFeedback.Modified = DateTime.UtcNow;       // Set modified date

            //     await db.SaveChangesAsync();

            //     return Results.NoContent(); // updated
            // });

        // Delete users
            feedback.MapDelete("/{id}", async (int id, HMS_Context db) => {

                await db.Feedbacks.Where(feedback => feedback.Id == id)          // select user to remove
                    .ExecuteDeleteAsync();                                   // execute

                return Results.NoContent();                                 // deletes or not, does not matter
            });

        return feedback;
    }
}
