using System;
using HMS_API.Data;
using HMS_API.Dtos.module;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS_API.Endpoints;

public class ModuleEndpoints
{
    private const string GetModuleEndpointName = "GetModule";
    private readonly ILogger<ModuleEndpoints> logger;

    // Constructor dependency injection
    public ModuleEndpoints()
    {
            logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<ModuleEndpoints>();
    }

    public RouteGroupBuilder MapModuleEndpoints(WebApplication app)
    {
        var module = app.MapGroup("modules")
                        .WithParameterValidation();

        // Requests for all modules
         module.MapGet("/", async (HMS_Context db) => {
            try
            {
                    var modules = await db.Modules
                    .Include(module => module.UserAccount)      // Include user entity to get info as in DTO
                    .Select(user => user.ToModuleSummaryDto())
                    .AsNoTracking()                              // Optimize query
                    .ToListAsync();                             // Asynchronous (async, await ToListAsync())

                    logger.LogInformation("Successfully fetched {count} modules.", modules.Count);
                    return Results.Ok(modules);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to fetch modules/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving modules.");
            }
        });


        // Get a specific module
        module.MapGet("/{id}", async (int id, HMS_Context db) => 

            {
                logger.LogInformation("Request to get module with ID: {id}", id);

                if (id < 1)
                {
                    logger.LogWarning("Invalid ID provided: {Id}. ID must be greater than 0.", id);
                    throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
                }

               Module? module = await db.Modules.FindAsync(id); // Find ID in db

                if(module == null)
                {       // log failure
                    logger.LogWarning("Module with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                // Log a success when the user is found
                logger.LogInformation("Module with ID {Id} successfully retrieved/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Ok(module.ToModuleDetailsDto());

            }).WithName(GetModuleEndpointName);
            


        // Post, create module
        module.MapPost("/", async (CreateModuleDto newModule, HMS_Context db) => 
        { 
            try
            {
                Module module = newModule.ToEntity();
                db.Modules.Add(module);
                await db.SaveChangesAsync();

                logger.LogInformation("Module with ID {ModuleId} successfully created/ Date/Time: {dateTime}.", module.Id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                return Results.CreatedAtRoute(GetModuleEndpointName, new { id = module.Id }, module.ToModuleDetailsDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create module.");
                return Results.Problem("An error occurred while creating the module.");
            }
        }).WithParameterValidation();

        // Put = update module
        module.MapPut("/{id}", async (int id, UpdateModuleDto updatedModule, HMS_Context db) => 
        {
            var existingModule = await db.Modules.FindAsync(id);

                if (existingModule == null) // Module must exist
                {
                    logger.LogWarning("Module with ID {ModuleId} not found for update/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }
            try
            {
                // set modified date
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                existingModule.Modified = currentDate;

                db.Entry(existingModule).CurrentValues.SetValues(updatedModule.ToEntity(id)); // Set updated values in database
                await db.SaveChangesAsync();

                logger.LogInformation("Module with ID {ModuleId} successfully updated/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Updated
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update module with ID {ModuleId}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while updating the module.");
            }
        }).WithParameterValidation();

        // Delete module
        module.MapDelete("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                var rowsAffected = await db.Modules
                                             .Where(module => module.Id == id) // Select module to remove
                                             .ExecuteDeleteAsync(); // Execute

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Attempted to delete module with ID {ModuleId}, but no module was found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }

                logger.LogInformation("Module with ID {ModuleId} successfully deleted/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.NoContent(); // Return NoContent
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting module with ID {ModuleId}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while deleting the module.");
            }
        });

        return module;
    }
}
