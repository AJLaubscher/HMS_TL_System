using System;
using HMS_API.Data;
using HMS_API.Dtos.userAccount;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public static class UserAccountEndpoints
{
    const string GetUserEndpointName = "GetUser";

    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var user = app.MapGroup("users").WithParameterValidation();

        // Request all users
        user.MapGet("/", async (HMS_Context db) => 
        await db.UserAccounts.Select(user => user.ToUserSummaryDto())
        .AsNoTracking()
        .ToListAsync());

        // get a specific User > pass user id
        user.MapGet("/{id}", async (int id, HMS_Context db) => {

        UserAccount? user = await db.UserAccounts.FindAsync(id);                // find id in db

        return user is null? Results.NotFound() : 
        Results.Ok(user.ToUserDetailsDto());                                    // if null return not found/ return ok if found
        }).WithName(GetUserEndpointName );

        // post, create user / add HMS_Context db as parameter for db mapping
        user.MapPost("/", async (CreateUserDto newUser, HMS_Context db) => { 

            UserAccount user = newUser.ToEntity();

            db.UserAccounts.Add(user);
            await db.SaveChangesAsync();

            return Results.CreatedAtRoute(GetUserEndpointName , new {id = user.Id}, user.ToUserDetailsDto()); 
        });


        // Put = update user
        user.MapPut("/{id}", async (int id, UpdateUserDto updatedUser, HMS_Context db) =>{


        var existingUser = await db.UserAccounts.FindAsync(id);       // find id in database

        if(existingUser == null) // user does not exist
        {
            return Results.NotFound();
        }

        db.Entry(existingUser).CurrentValues.SetValues(updatedUser.ToEntity(id)); // set updated values in database
        await db.SaveChangesAsync();

        return Results.NoContent(); // updated
        });

        // Delete users
        user.MapDelete("/{id}", async (int id, HMS_Context db) => {

            await db.UserAccounts.Where(user => user.Id == id)          // select user to remove
                .ExecuteDeleteAsync();                       // execute

            return Results.NoContent();} // deletes if exist, if not existing does not matter
        );
                        
        return user;
    }

}
