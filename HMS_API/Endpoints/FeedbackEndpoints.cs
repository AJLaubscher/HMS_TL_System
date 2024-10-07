using System;
using System.Security.Claims;
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
                          .WithParameterValidation()
                          .RequireAuthorization();

        // Requests for feedback
        feedback.MapGet("/", async (HMS_Context db, ClaimsPrincipal account) => 
        {
            try
            {
                if(VerifyAdminStudentClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role
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
        }).RequireAuthorization();

        // Get specific feedback
        feedback.MapGet("/{id}", async (int id, HMS_Context db, ClaimsPrincipal account) => 
        {
            try
            {
                if(VerifyAdminStudentClaim(account) == false) // Check if the user has the Admin or student role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role

                Feedback? feedback = await db.Feedbacks.FindAsync(id);

                return feedback is null ? Results.NotFound() : Results.Ok(feedback.ToFeedbackDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch feedback with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving feedback.");
            }
        }).RequireAuthorization().WithName(GetFeedbackEndpointName);

        // Post feedback
        feedback.MapPost("/", async (CreateFeedbackDto newFeedback, HMS_Context db, ClaimsPrincipal account) => 
        {
            try
            {
                if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role

                Feedback feedback = newFeedback.ToEntity();
                // force date creation
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                feedback.Created = currentDate;
                feedback.ReturnDate = currentDate;
                feedback.Modified = currentDate;

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
        }).RequireAuthorization().WithParameterValidation();

        // Put = update feedback
        feedback.MapPut("/{id}", async (int id, UpdateFeedbackDto updateFeedback, HMS_Context db, ClaimsPrincipal account) => 
        {
            if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                return Results.Forbid(); // Return 403 Forbidden if neither role

            var existingFeedback = await db.Feedbacks.FindAsync(id);

            if (existingFeedback == null) // Feedback must exist
            {
                logger.LogWarning("Feedback with ID {id} not found for update/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NotFound();
            }
            try
            {
                // inject updated information
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                existingFeedback.Comment = updateFeedback.Comment;
                existingFeedback.ReturnDate = currentDate;
                existingFeedback.MarkAchieved = updateFeedback.MarkAchieved;
                existingFeedback.Modified = currentDate;
                existingFeedback.Deleted = updateFeedback.Deleted;

                //db.Entry(existingFeedback).CurrentValues.SetValues(updateFeedback.ToEntity(id)); // Set updated values in database
                await db.SaveChangesAsync();

                logger.LogInformation("Feedback with ID {Id} successfully updated/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Updated
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update feedback with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating feedback.");
            }
        }).RequireAuthorization();

        // Delete feedback
        feedback.MapDelete("/{id}", async (int id, HMS_Context db, ClaimsPrincipal account) => 
        {
            try
            {
                if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                return Results.Forbid(); // Return 403 Forbidden if neither role

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
        }).RequireAuthorization();

        return feedback;
    }
    private bool VerifyAdminLecturerClaim(ClaimsPrincipal user) // verify admin or lecturer
    {
        bool verify = false;
        if (user.HasClaim(claim => 
            claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && 
                (claim.Value == "Admin" || claim.Value == "Lecturer")))
                {
                    verify = true; 
                }
        return verify;
    }
    private bool VerifyAdminStudentClaim(ClaimsPrincipal user) // verify admin or student
    {
        bool verify = false;
        if (user.HasClaim(claim => 
            claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && 
                (claim.Value == "Admin" || claim.Value == "Student")))
                {
                    verify = true; 
                }
        return verify;
    }
}
