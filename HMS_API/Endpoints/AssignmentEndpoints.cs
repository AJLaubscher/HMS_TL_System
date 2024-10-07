using System;
using System.Security.Claims;
using HMS_API.Data;
using HMS_API.Dtos.assignment;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public class AssignmentEndpoints
{
    const string GetAssignmentEndpointName = "GetAssignment";

    // Constructor for Dependency Injection
    private readonly ILogger<AssignmentEndpoints> logger;

    public AssignmentEndpoints()
    {
            logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<AssignmentEndpoints>();
    }

    public RouteGroupBuilder MapAssignmentEndpoints(WebApplication app)
    {
        var assignment = app.MapGroup("assignments")
                            .WithParameterValidation()
                            .RequireAuthorization();

        // Requests for all assignments
        assignment.MapGet("/", async (HMS_Context db, ClaimsPrincipal account) => 
        {
            try
            {
                if(VerifyAdminLecturerClaim(account) == false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role

                var assingments = await db.Assignments
                    .Include(assignment => assignment.Module) // Include module entity to get info as in DTO
                    .Select(assignment => assignment.ToAssignmentSummaryDto())
                    .AsNoTracking() // Optimize query
                    .ToListAsync(); // Asynchronous (async, await ToListAsync())

                logger.LogInformation("Successfully fetched {count} assignments.", assingments.Count);
                return Results.Ok(assingments);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to fetch assignments/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving assignments.");
            }
        });


        // Get a specific assignment
        assignment.MapGet("/{id}", async (int id, HMS_Context db, ClaimsPrincipal account) => {
            try
            {
                if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role

                if (id < 1)
                {
                    logger.LogWarning("Invalid ID provided: {Id}. ID must be greater than 0.", id);
                    throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
                }

                Assignment? assignment = await db.Assignments.FindAsync(id); // Find assignment by id in db

                if(assignment == null)
                {
                    logger.LogWarning("Assignment with ID {Id} not found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }

                // Log a success when the assignment is found
                logger.LogInformation("Assignment with ID {Id} successfully retrieved/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Ok(assignment.ToAssignmentDetailsDto());

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to fetch assignment/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving assignment.");
            }

        }).WithName(GetAssignmentEndpointName);




        // Post, create assignment
        assignment.MapPost("/", async (CreateAssignmentDto newAssignment, HMS_Context db, ClaimsPrincipal account) => { 

            if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                return Results.Forbid(); // Return 403 Forbidden if neither role 

            // Data validation
            if (newAssignment == null)
            {
                logger.LogWarning("New assignment has no values.");
                return Results.BadRequest("Invalid assignment data.");
            }

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

            if (newAssignment.OpenDate < currentDate)
            {
                logger.LogWarning("OpenDate is before the current system date.");
                return Results.BadRequest("The open date must be today or later.");
            }

            if (newAssignment.DueDate <= newAssignment.OpenDate)
            {
                logger.LogWarning("DueDate is not after the OpenDate.");
                return Results.BadRequest("The due date must be after the open date.");
            }

            // Continue to pass data to database
            try
            {
                Assignment assignment = newAssignment.ToEntity();

                //force correct dates
                assignment.Created = currentDate;
                assignment.Modified = currentDate;
                assignment.SubPath = "Wait for path";

                // Save the assignment to generate the ID
                db.Assignments.Add(assignment);
                await db.SaveChangesAsync(); // This generates the Assignment ID

                // Define the default base path (e.g., Documents\HMS_Assignments)
                var baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HMS_Assignments");

                // after ID is generated, append it to the subpath
                var modId = newAssignment.ModID; 
                var fullPath = Path.Combine(baseDirectory, modId.ToString(), assignment.Id.ToString());

                // Ensure the directory exists
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Update the assignment with the new subpath and save the changes
                assignment.SubPath = fullPath; // Assign the updated subpath
                db.Assignments.Update(assignment); // Update the assignment in the database
                await db.SaveChangesAsync();

                logger.LogInformation("Assignment successfully created/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Results.CreatedAtRoute(GetAssignmentEndpointName, new { id = assignment.Id }, assignment.ToAssignmentDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create assignment/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while creating the assignment.");
            }
        });

        // Put = update assignment
        assignment.MapPut("/{id}", async (int id, UpdateAssignmentDto updatedAssignment, HMS_Context db, ClaimsPrincipal account) => {

            if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                return Results.Forbid(); // Return 403 Forbidden if neither role

            var existingAssignment = await db.Assignments.FindAsync(id);

            if (existingAssignment == null) // Assignment does not exist
            {
                return Results.NotFound();
            }

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now); // current date

            if(updatedAssignment.OpenDate < currentDate)
            {
                logger.LogWarning("OpenDate is before the current system date.");
                return Results.BadRequest("The open date must be today or later.");
            }
            if (updatedAssignment.DueDate <= updatedAssignment.OpenDate)
            {
                logger.LogWarning("DueDate is not after the OpenDate.");
                return Results.BadRequest("The due date must be after the open date.");
            } 

            try
            {
                //db.Entry(existingAssignment).CurrentValues.SetValues(updatedAssignment.ToEntity(id)); // Set updated values in database

                // set modified to current date
                existingAssignment.Modified = currentDate;

                // Build the subpath using modId and assignmentId
                var modId = updatedAssignment.ModID; 
                // default base path (e.g., Documents\HMS_Assignments)
                var baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HMS_Assignments");
                // Build the full path using baseDirectory, modId, and assignmentId
                var fullPath = Path.Combine(baseDirectory, modId.ToString(), id.ToString());

                // Ensure the directory exists
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Update the assignment's values and save to db
                existingAssignment.ModID = updatedAssignment.ModID;
                existingAssignment.Title = updatedAssignment.Title;
                existingAssignment.Instructions = updatedAssignment.Instructions;
                existingAssignment.OpenDate = updatedAssignment.OpenDate;
                existingAssignment.DueDate = updatedAssignment.DueDate;
                existingAssignment.MaxMarks = updatedAssignment.MaxMarks;
                existingAssignment.SubPath = fullPath;
                existingAssignment.Modified = currentDate;
                existingAssignment.Deleted = updatedAssignment.Deleted;

                await db.SaveChangesAsync();

                logger.LogInformation("Assignment with ID {Id} successfully updated/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Results.NoContent(); // Updated successfully
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to update assignment ID {id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating assignment.");
            }
        });

        // Delete assignment
        assignment.MapDelete("/{id}", async (int id, HMS_Context db, ClaimsPrincipal account) => {
            try
            {
                if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role
                // Attempt to delete the assignment with the given ID
                var rowsAffected = await db.Assignments.Where(assignment => assignment.Id == id)
                                                    .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    // Log and return NotFound if no assignment was deleted
                    logger.LogWarning("Failed to delete. Assignment with ID {Id} not found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound("Assignment not found.");
                }

                // Log success and return NoContent if the deletion was successful
                logger.LogInformation("Assignment with ID {Id} successfully deleted/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                logger.LogError(ex, "An error occurred while deleting assignment with ID {Id}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while deleting the assignment.");
            }
        });

        return assignment;
    }
    private bool VerifyAdminLecturerClaim(ClaimsPrincipal user) // verify admin or lecturer
    {
        bool verify = false;
        if (user.HasClaim(claim => 
            claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && 
                (claim.Value == "Admin" || claim.Value == "Lecturer")))
                {
                    verify = true; // Return 403 Forbidden if neither role
                }
        return verify;
    }
}
