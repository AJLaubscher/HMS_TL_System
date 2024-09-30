using System;
using HMS_API.Data;
using HMS_API.Dtos.enrollment;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS_API.Endpoints;

public class EnrolmentEndpoints
{
    private const string GetEnrolmentEndpointName = "GetEnrolment";
    private readonly ILogger<EnrolmentEndpoints> logger;

    // Constructor for Dependency Injection
    public EnrolmentEndpoints()
    {
        logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<EnrolmentEndpoints>();
    }

    public RouteGroupBuilder MapEnrolmentEndpoints(WebApplication app)
    {
        var enrolment = app.MapGroup("enrolments")
                           .WithParameterValidation();

        // Requests for all enrolments
        enrolment.MapGet("/", async (HMS_Context db) => {
            try
            {
                    var enrolments =  await db.Enrolments
            			.Include(e => e.Module) // Include module entity to get info as in DTO
            			.Include(e => e.UserAccount) // Include user entity to get info as in DTO
            			.Select(e => e.ToEnrolmentSummaryDto())
            			.AsNoTracking() // Optimize query
            			.ToListAsync(); // Asynchronous (async, await ToListAsync())

                    logger.LogInformation("Successfully fetched {count} enrolments.", enrolments.Count);
                    return Results.Ok(enrolments);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to fetch enrolments/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving enrolments.");
            }
        });

        // Get a specific enrolment
        enrolment.MapGet("/{modId}/{studId}", async (int modId, int studId, HMS_Context db) => 
            {
                logger.LogInformation("Request to get enrolment with ID: {modId}, {studId}.",  modId, studId);

                if ((modId < 1) || (studId < 1))
                {
                    logger.LogWarning("Enrolment with ID {modId}, {studId}. ID's must be greater than 0.", modId, studId);
                    throw new ArgumentOutOfRangeException(nameof(modId), nameof(studId), "The id must be greater than 0!");
                }

               Enrolment? enrolment = await db.Enrolments.FindAsync(modId, studId); // Find id in db

                if(enrolment == null)
                {       // log failure
                    logger.LogWarning("Enrolment with ID {modId}, {studId} not found/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }

                // Log a success when the user is found
                logger.LogInformation("Enrolment with ID {modId}, {studId} successfully retrieved/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Ok(enrolment.ToEnrolmentDetailsDto());

            }).WithName(GetEnrolmentEndpointName);

            // // Post: create enrolment
            enrolment.MapPost("/", async (CreateEnrolmentDto newEnrolment, HMS_Context db) => 
            {
                try
                {
                    // Ensure both ModID and StudID are present in the request
                    if (newEnrolment.ModID == 0 || newEnrolment.StudID == 0)
                    {
                        logger.LogWarning("Module ID or Student ID is missing/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                        return Results.BadRequest("Module ID and Student ID must be provided.");
                    }

                    // Check if the enrolment already exists
                    var existingEnrolment = await db.Enrolments
                                                    .FirstOrDefaultAsync(e => e.ModID == newEnrolment.ModID && e.StudID == newEnrolment.StudID);
                    
                    if (existingEnrolment != null)
                    {
                        logger.LogWarning("Enrolment with Module ID {ModId} and Student ID {StudId} already exists/ Date/Time: {dateTime}.", 
                                        newEnrolment.ModID, newEnrolment.StudID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                        return Results.Conflict("Enrolment already exists.");
                    }
                    
                        // Create a new enrolment entity
                        Enrolment enrolment = newEnrolment.ToEntity();
                        DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow); // set enrol year
                        enrolment.Enrol_year = currentDate;
                        enrolment.Created = currentDate;  // set creation date


                    // Add the enrolment to the database
                    db.Enrolments.Add(enrolment);
                    await db.SaveChangesAsync();

                    logger.LogInformation("Enrolment with Module ID {ModId} and Student ID {StudId} successfully created/ Date/Time: {dateTime}.", 
                                        enrolment.ModID, enrolment.StudID, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Return the newly created enrolment details
                    return Results.CreatedAtRoute("GetEnrolment", new { modId = enrolment.ModID, studId = enrolment.StudID }, enrolment);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to create enrolment/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.Problem("An error occurred while creating the enrolment.");
                }
            });

        // Put = update enrolment
        enrolment.MapPut("/{modId}/{studId}", async (int modId, int studId, UpdateEnrolmentDto updatedEnrolment, HMS_Context db) => 
        {
                var existingEnrolment = await db.Enrolments.FindAsync(modId, studId);

                if (existingEnrolment == null) // Enrolment must exist
                {
                    logger.LogWarning("Enrolment with Module ID {ModId} and Student ID {StudId} not found for update/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }
            try
            {
                // set modified date
                existingEnrolment.Enrol_year = updatedEnrolment.Enrol_year;
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                existingEnrolment.Modified = currentDate;
                existingEnrolment.Deleted = updatedEnrolment.Deleted;

                //db.Entry(existingEnrolment).CurrentValues.SetValues(updatedEnrolment.ToEntity(modId, studId)); // Set updated values in database
                await db.SaveChangesAsync();

                logger.LogInformation("Enrolment with Module ID {ModId} and Student ID {StudId} successfully updated/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Updated
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update enrolment with Module ID {ModId} and Student ID {StudId}/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating the enrolment.");
            }
        });

        // Delete enrolment
        enrolment.MapDelete("/{modId}/{studId}", async (int modId, int studId, HMS_Context db) => 
        {
            try
            {
                var rowsAffected = await db.Enrolments
                                            .Where(e => e.ModID == modId && e.StudID == studId) // Select enrolment to remove
                                            .ExecuteDeleteAsync(); // Execute

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Attempted to delete enrolment with Module ID {ModId} and Student ID {StudId}, but no enrolment was found.", modId, studId);
                    return Results.NotFound();
                }

                logger.LogInformation("Enrolment with Module ID {ModId} and Student ID {StudId} successfully deleted/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Return NoContent
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting enrolment with Module ID {ModId} and Student ID {StudId}/ Date/Time: {dateTime}.", modId, studId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while deleting the enrolment.");
            }
        });

        return enrolment;
    }
}