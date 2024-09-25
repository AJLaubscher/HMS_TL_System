using System;
using HMS_API.Data;
using HMS_API.Dtos.submission;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS_API.Endpoints;

public class SubmissionEndpoints
{
    const string GetSubmissionEndpointName = "GetSubmission";
    private readonly ILogger<SubmissionEndpoints> logger;

    // Constructor for Dependency Injection
    public SubmissionEndpoints()
    {
            logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<SubmissionEndpoints>();
    }

    public RouteGroupBuilder MapSubmissionEndpoints(WebApplication app)
    {
        var submission = app.MapGroup("submissions")
                            .WithParameterValidation();

        // Requests for all submissions
        submission.MapGet("/", async (HMS_Context db) => 
        {
            try
            {
                var submissions = await db.Submissions
                    .Include(submission => submission.UserAccount)              // Include UserAccount entity to get info as in DTO
                    .Include(submission => submission.Assignment)               // Include Assignment entity to get info as in DTO
                    .Select(submission => submission.ToSubmissionSummaryDto())
                    .AsNoTracking()                                           // Optimize query
                    .ToListAsync();                                          // Asynchronous (async, await ToListAsync())

                logger.LogInformation("Successfully fetched {count} submissions.", submissions.Count);
                return Results.Ok(submissions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch submissions/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving submissions.");
            }
        });

        // Get a specific submission
        submission.MapGet("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                if (id < 1)
                {
                    logger.LogWarning("Invalid ID provided: {Id}. ID must be greater than 0.", id);
                    throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
                }

                Submission? submission = await db.Submissions.FindAsync(id); // Find submission by id in db

                if (submission == null)
                {
                    logger.LogWarning("Submission with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                // Log a success when the submission is found
                logger.LogInformation("Submission with ID {Id} successfully retrieved/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Ok(submission.ToSubmissionDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch submission/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving submission.");
            }
        }).WithName(GetSubmissionEndpointName);

        // Post, create submission
        submission.MapPost("/", async (CreateSubmissionDto newSubmission, HMS_Context db) => 
        {
            
            if (newSubmission == null)
            {
                logger.LogWarning("New submission has no values.");
                return Results.BadRequest("Invalid submission data.");
            }

            try
            {
                Submission submission = newSubmission.ToEntity();
                db.Submissions.Add(submission); // Add to db
                await db.SaveChangesAsync();

                logger.LogInformation("Submission successfully created/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.CreatedAtRoute(GetSubmissionEndpointName, new { id = submission.Id }, submission.ToSubmissionDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create submission.");
                return Results.Problem("An error occurred while creating submission.");
            }
        }).WithParameterValidation();

        // Put = update submission
        submission.MapPut("/{id}", async (int id, UpdateSubmissionDto updatedSubmission, HMS_Context db) => 
        {
            var  existingSubmission = await db.Submissions.FindAsync(id);

                if (existingSubmission == null) // Submisson must exist
                {
                    logger.LogWarning("Submission with ID {id} not found for update.", id);
                    return Results.NotFound();
                }
            try
            {
                // change date on modified
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                existingSubmission.Modified = currentDate;

                db.Entry(existingSubmission).CurrentValues.SetValues(updatedSubmission.ToEntity(id)); // Set updated values in database
                await db.SaveChangesAsync();

                logger.LogInformation("Submission with ID {Id} successfully updated/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Updated successfully
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update submission/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating submission.");
            }
        }).WithParameterValidation();

        // Delete submission
        submission.MapDelete("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                // Attempt to delete the submission with the given ID
                var rowsAffected = await db.Submissions.Where(submission => submission.Id == id)
                                                        .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    // Log and return NotFound if no submission was deleted
                    logger.LogWarning("Failed to delete. Submission with ID {Id} not found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound("Submission not found.");
                }

                // Log success and return NoContent if the deletion was successful
                logger.LogInformation("Submission with ID {Id} successfully deleted/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                logger.LogError(ex, "An error occurred while deleting submission with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while deleting the submission.");
            }
        });

        return submission;
    }
}
