using System;
using HMS_API.Data;
using HMS_API.Dtos.enrollment;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class EnrolmentEndpoints
{
    const string GetEnrolmentEndpointName = "GetEnrolment";
    public static RouteGroupBuilder MapEnrolmentEndpoints(this WebApplication app)
    {
        var enrolment = app.MapGroup("enrolments")
                        .WithParameterValidation();

                // requests for all modules
            enrolment.MapGet("/", async (HMS_Context db) => 
            await db.Enrolments
            .Include(enrolment => enrolment.Module)                  // include module entity to get info as in DTO
            .Include(enrolment => enrolment.UserAccount)            // include user entity to get info as in DTO
            .Select(enrolment => enrolment.ToEnrolmentSummaryDto())
            .AsNoTracking()                                         // optimize query
            .ToListAsync());                                        // asynchronous (async, await ToListAsync())

            // get a specific enrollment
            enrolment.MapGet("/{modId}/{studId}", async (int modId, int studId, HMS_Context db) => {

               Enrolment? enrolment = await db.Enrolments.FindAsync(modId, studId); // find id in db

                return enrolment is null? Results.NotFound() : Results.Ok(enrolment.ToEnrolmentDetailsDto()); // if null return not found/ return ok if found
                }
            ).WithName(GetEnrolmentEndpointName);


            // post, create user / add HMS_Context db as parameter for db mapping
            enrolment.MapPost("/", async (CreateEnrolmentDto newEnrolment, HMS_Context db) => { 

            Enrolment enrolment = newEnrolment.ToEntity();

            db.Enrolments.Add(enrolment);
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetEnrolmentEndpointName , new {modId = enrolment.ModID, studId = enrolment.StudID}, enrolment.ToEnrolmentDetailsDto()); 
            });


            // Put = update module
            enrolment.MapPut("/{modId}/{studId}", async (int modId, int studId, UpdateEnrolmentDto updatedEnrolment, HMS_Context db) =>{

                var existingEnrolment = await db.Enrolments.FindAsync(modId, studId);

                if(existingEnrolment == null) // submission does not exist
                {
                    return Results.NotFound();
                }

                db.Entry(existingEnrolment).CurrentValues.SetValues(updatedEnrolment.ToEntity(modId, studId)); // set updated values in database
                await db.SaveChangesAsync();

                return Results.NoContent(); // updated
            });


            // another way of updating if this does not work correctly
            // PUT request to update enrollment
            // enrolment.MapPut("/{modId}/{studId}", async (int modId, int studId, UpdateEnrolmentDto updatedEnrolment, HMS_Context db) =>
            // {
            //     // Find the existing enrollment using composite keys (modId, studId)
            //     var existingEnrolment = await db.Enrolments.FindAsync(modId, studId);

            //     // If not found, return 404
            //     if (existingEnrolment == null)
            //     {
            //         return Results.NotFound();
            //     }

            //     // Map updated DTO to the existing enrollment entity (without changing modId and studId)
            //     existingEnrolment.Enrol_year = updatedEnrolment.Enrol_year;
            //     existingEnrolment.Modified = updatedEnrolment.Modified;
            //     existingEnrolment.Deleted = updatedEnrolment.Deleted;

            //     // Save the updated entity
            //     await db.SaveChangesAsync();

            //     // Return 204 No Content (successful update)
            //     return Results.NoContent();
            // });

            // Delete module
            enrolment.MapDelete("/{modId}/{studId}", async (int modId, int studId, HMS_Context db) => {

                    await db.Enrolments.Where(enrolment => enrolment.ModID == modId && enrolment.StudID == studId)               // select enrolment to remove
                        .ExecuteDeleteAsync();                                       // execute

                    return Results.NoContent();                                     // deletes or not, does not matter
                });


        return enrolment;
    }
}
