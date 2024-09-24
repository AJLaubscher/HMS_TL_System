using System;
using HMS_API.Data;
using HMS_API.Dtos.assignment;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class AssignmentEndpoints
{
    const string GetAssignmentEndpointName = "GetAssignment";
    public static RouteGroupBuilder MapAssignmentEndpoints(this WebApplication app)
    {
        var assignment = app.MapGroup("assignments")
                        .WithParameterValidation();

                // requests for all modules
            assignment.MapGet("/", async (HMS_Context db) => 
            await db.Assignments
            .Include(assignment => assignment.Module)               // include module entity to get info as in DTO
            .Select(assignment => assignment.ToAssignmentSummaryDto())
            .AsNoTracking()                                         // optimize query
            .ToListAsync());                                        // asynchronous (async, await ToListAsync())

            // get a specific enrollment
            assignment.MapGet("/{id}", async (int id, HMS_Context db) => {

            if(id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
            }

               Assignment? assignment = await db.Assignments.FindAsync(id); // find id in db

                return assignment is null? Results.NotFound() : Results.Ok(assignment.ToAssignmentDetailsDto()); // if null return not found/ return ok if found
                }
            ).WithName(GetAssignmentEndpointName);


            // post, create user / add HMS_Context db as parameter for db mapping
            assignment.MapPost("/", async (CreateAssignmentDto newAssignment, HMS_Context db) => { 

            Assignment assignment = newAssignment.ToEntity();

            db.Assignments.Add(assignment);
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetAssignmentEndpointName , new {id = assignment.Id}, assignment.ToAssignmentDetailsDto()); 
            });


            // Put = update module
            assignment.MapPut("/{id}", async (int id, UpdateAssignmentDto updatedAssignment, HMS_Context db) =>{

                var existingAssignment = await db.Assignments.FindAsync(id);

                if(existingAssignment == null) // assignment does not exist
                {
                    return Results.NotFound();
                }

                db.Entry(existingAssignment).CurrentValues.SetValues(updatedAssignment.ToEntity(id)); // set updated values in database
                await db.SaveChangesAsync();

                return Results.NoContent(); // updated
            });


            // code if put does not work as intended
            // Put = update assignment
            // assignment.MapPut("/{id}", async (int id, UpdateAssignmentDto updatedAssignment, HMS_Context db) =>
            // {
            //     var existingAssignment = await db.Assignments.FindAsync(id);

            //     if (existingAssignment == null) // assignment does not exist
            //     {
            //         return Results.NotFound();
            //     }

            //     // Map updated fields explicitly
            //     existingAssignment.Title = updatedAssignment.Title; 
            //     existingAssignment.Description = updatedAssignment.Description; 
            //     existingAssignment.DueDate = updatedAssignment.DueDate; 
            //     existingAssignment.Modified = DateTime.UtcNow; 

            //     await db.SaveChangesAsync();

            //     return Results.NoContent(); // updated
            // });

            // Delete module
            assignment.MapDelete("/{id}", async (int id, HMS_Context db) => {

                    await db.Assignments.Where(assignment => assignment.Id == id)    // select assignment to remove
                        .ExecuteDeleteAsync();                                       // execute

                    return Results.NoContent();                                     // deletes or not, does not matter
                });


        return assignment;
    }
}
