using System;
using HMS_API.Data;
using HMS_API.Dtos.module;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class ModuleEndpoints
{
    const string GetModuleEndpointName = "GetModule";
    public static RouteGroupBuilder MapModuleEndpoints(this WebApplication app)
    {
        var module = app.MapGroup("modules")
                        .WithParameterValidation();

                // requests for all modules
            module.MapGet("/", async (HMS_Context db) => 
            await db.Modules
            .Include(module => module.UserAccount)                    // include user entity to get info as in DTO
            .Select(user => user.ToModuleSummaryDto())
            .AsNoTracking()                                         // optimize query
            .ToListAsync());                                        // asynchronous (async, await ToListAsync())

            // get a specific module
            module.MapGet("/{id}", async (int id, HMS_Context db) => {

            if(id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
            }

               Module? module = await db.Modules.FindAsync(id); // find id in db

                return module is null? Results.NotFound() : Results.Ok(module.ToModuleDetailsDto()); // if null return not found/ return ok if found
                }
            ).WithName(GetModuleEndpointName);


            // post, create user / add HMS_Context db as parameter for db mapping
            module.MapPost("/", async (CreateModuleDto newModule, HMS_Context db) => { 

            Module module = newModule.ToEntity();

            db.Modules.Add(module);
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetModuleEndpointName , new {id = module.Id}, module.ToModuleDetailsDto()); 
            });


            // Put = update module
            module.MapPut("/{id}", async (int id, UpdateModuleDto updatedModule, HMS_Context db) =>{

                var existingModule = await db.Modules.FindAsync(id);

                if(existingModule == null) // submission does not exist
                {
                    return Results.NotFound();
                }

                db.Entry(existingModule).CurrentValues.SetValues(updatedModule.ToEntity(id)); // set updated values in database
                await db.SaveChangesAsync();

                return Results.NoContent(); // updated
            });

            // Delete module
            module.MapDelete("/{id}", async (int id, HMS_Context db) => {

                    await db.Modules.Where(module => module.Id == id)               // select submission to remove
                        .ExecuteDeleteAsync();                                       // execute

                    return Results.NoContent();                                     // deletes or not, does not matter
                });


        return module;
    }
}
