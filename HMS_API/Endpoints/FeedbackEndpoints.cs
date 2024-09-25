using System;
using HMS_API.Data;
using HMS_API.Dtos.feedback;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS_API.Endpoints;

public class FeedbackEndpoints
{
    const string GetFeedbackEndpointName = "GetFeedback";
    private readonly ILogger<FeedbackEndpoints> logger;

    // Constructor for Dependency Injection
    public FeedbackEndpoints()
    {
            logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<FeedbackEndpoints>();
    }

    public RouteGroupBuilder MapFeedbackEndpoints(WebApplication app)
    {
        var feedback = app.MapGroup("feedback")
                          .WithParameterValidation();

        // Requests for feedback
        feedback.MapGet("/", async (HMS_Context db) => 
        {
            try
            {
                var feedbacks = await db.Feedbacks
                    .Include(f => f.Submission) // Include submission entity to get info as in DTO
                    .Include(f => f.UserAccount) // Include user entity to get info as in DTO
                    .Select(f => f.ToFeedbackSummaryDto())
                    .AsNoTracking() // Optimize query
                    .ToListAsync();

                logger.LogInformation("Successfully fetched {count} feedbacks.", feedbacks.Count);
                return Results.Ok(feedbacks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch feedbacks/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving feedbacks.");
            }
        });

        // Get specific feedback
        feedback.MapGet("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                Feedback? feedback = await db.Feedbacks.FindAsync(id);

                return feedback is null ? Results.NotFound() : Results.Ok(feedback.ToFeedbackDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch feedback with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving feedback.");
            }
        }).WithName(GetFeedbackEndpointName);

        // Post feedback
        feedback.MapPost("/", async (CreateFeedbackDto newFeedback, HMS_Context db) => 
        {
            try
            {
                Feedback feedback = newFeedback.ToEntity();
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();

                logger.LogInformation("Feedback successfully created with ID {Id}/ Date/Time: {dateTime}.", feedback.Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.CreatedAtRoute(GetFeedbackEndpointName, new { id = feedback.Id }, feedback.ToFeedbackDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create feedback.");
                return Results.Problem("An error occurred while creating feedback.");
            }
        }).WithParameterValidation();

        // Put = update feedback
        feedback.MapPut("/{id}", async (int id, UpdateFeedbackDto updateFeedback, HMS_Context db) => 
        {
                var existingFeedback = await db.Feedbacks.FindAsync(id);

                if (existingFeedback == null) // Feedback must exst
                {
                    logger.LogWarning("Feedback with ID {id} not found for update/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }
            try
            {
                // change date on modified
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                existingFeedback.Modified = currentDate;

                db.Entry(existingFeedback).CurrentValues.SetValues(updateFeedback.ToEntity(id)); // Set updated values in database
                await db.SaveChangesAsync();

                logger.LogInformation("Feedback with ID {Id} successfully updated/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Updated
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update feedback with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating feedback.");
            }
        }).WithParameterValidation();

        // Delete feedback
        feedback.MapDelete("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                var rowsAffected = await db.Feedbacks.Where(f => f.Id == id).ExecuteDeleteAsync();
                if (rowsAffected == 0)
                {
                    logger.LogWarning("Failed to delete. Feedback with ID {Id} not found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound("Feedback not found.");
                }

                logger.LogInformation("Feedback with ID {Id} successfully deleted/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting feedback with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while deleting feedback.");
            }
        });

        return feedback;
    }
}