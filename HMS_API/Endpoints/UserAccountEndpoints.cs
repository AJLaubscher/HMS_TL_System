using System;
using HMS_API.Data;
using HMS_API.Dtos.userAccount;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Endpoints;

public class UserAccountEndpoints
{

    private readonly ILogger<UserAccountEndpoints> logger;
     
     // Constructor for Dependency Injection
    public UserAccountEndpoints()
    {
            logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<UserAccountEndpoints>();
    }

    const string GetUserEndpointName = "GetUser";

    public RouteGroupBuilder MapUserEndpoints(WebApplication app)
    {
        var user = app.MapGroup("users").WithParameterValidation();


        // Request all users
        user.MapGet("/", async (HMS_Context db) => {
            try
            {
                    var users = await db.UserAccounts
                     .Select(user => user.ToUserSummaryDto())
                     .AsNoTracking()
                     .ToListAsync();

                    logger.LogInformation("Successfully fetched {count} users/ Date/Time: {dateTime}.", users.Count, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.Ok(users);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to fetch assignments/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Problem("An error occurred while retrieving assignments.");
            }
        });


        // Get a specific user by ID
        user.MapGet("/{id}", async (int id, HMS_Context db) => 
            {
                logger.LogInformation("Request to get user with ID: {id}", id);

                if (id < 1)
                {
                    logger.LogWarning("Invalid ID provided: {Id}. ID must be greater than 0.", id);
                    throw new ArgumentOutOfRangeException(nameof(id), "The id must be greater than 0!");
                }

               UserAccount? user = await db.UserAccounts.FindAsync(id); // find id in db

                if(user == null)
                {       // log failure
                    logger.LogWarning("User with ID {Id} not found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }

                // Log a success when the user is found
                logger.LogInformation("User with ID {Id} successfully retrieved/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                return Results.Ok(user.ToUserDetailsDto());

            }).WithName(GetUserEndpointName);



        // post new user & hash password
            user.MapPost("/", async (CreateUserDto newUser,  HMS_Context db) => {
                try
                {
                    // Create a new user entity from the DTO
                    UserAccount user = newUser.ToEntity();

                    // Hash user password and store in DB
                    string passwordTohash = user.UserPassword;
                    user.UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordTohash, 13);
                    // force current dates to database
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    user.Created = currentDate;
                    user.Modified = currentDate;
                    
                    // To verify password =  //user.UserPassword = BCrypt.Net.BCrypt.EnhancedVerify(passwordToVerify, passwordTohash);

                    // Add user to the database
                    db.UserAccounts.Add(user);
                    await db.SaveChangesAsync();

                    // Log success
                    logger.LogInformation("User with ID {UserId} successfully created/ Date/Time: {dateTime}.",user.Id , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Return the created user details
                    return Results.CreatedAtRoute(GetUserEndpointName, new { id = user.Id }, user.ToUserDetailsDto());
                }
                catch (Exception ex)
                {
                    // Log the exception and failure
                    logger.LogError(ex, "An error occurred while creating a new user/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Return an error response
                    return Results.Problem("An error occurred while creating the user");
                }
            }).WithParameterValidation();

// Put: update an existing user
user.MapPut("/{id}", async (int id, UpdateUserDto updatedUser, HMS_Context db) => 
{
    var existingUser = await db.UserAccounts.FindAsync(id);

    if (existingUser == null) // Find user first
    {
        logger.LogWarning("User with ID {UserId} not found for update.", id);
        return Results.NotFound();
    }

    try
    {
        // Check if a new password is provided and hash it
        if (!string.IsNullOrEmpty(updatedUser.UserPassword))
        {
            existingUser.UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.UserPassword, 13);
        }

        // Map other properties from the DTO to the existing user
        existingUser.Username = updatedUser.Username;
        existingUser.FName = updatedUser.FName;
        existingUser.LName = updatedUser.LName;
        existingUser.UserRole = updatedUser.UserRole;
        existingUser.Deleted = updatedUser.Deleted;

        // Update the modified date
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        existingUser.Modified = currentDate;

        // Save changes
        await db.SaveChangesAsync();

        // Log success
        logger.LogInformation("User with ID {UserId} successfully updated / Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        // Log the exception and failure
        logger.LogError(ex, "An error occurred while updating user with ID {UserId}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

        // Return an error response
        return Results.Problem("An error occurred while updating the user.");
    }
}).WithParameterValidation();


        // Delete: delete a user by ID
        user.MapDelete("/{id}", async (int id, HMS_Context db) => 
        {
            try
            {
                var rowsAffected = await db.UserAccounts
                                            .Where(user => user.Id == id)
                                            .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Attempted to delete user with ID {UserId}, but no user was found/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Results.NotFound();
                }

                // Log success
                logger.LogInformation("User with ID {UserId} successfully deleted/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception and failure
                logger.LogError(ex, "An error occurred while deleting user with ID {UserId}/ Date/Time: {dateTime}.", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Return an error response
                return Results.Problem("An error occurred while deleting the user.");
            }
        });

        // register a user
        user.MapPost("/register", async (CreateUserDto newUser, HMS_Context db) => 
        {
                
            try
            {
                // Create a new user entity from the DTO
                UserAccount user = newUser.ToEntity();

                // Hash user password and store in DB
                string passwordTohash = user.UserPassword;
                user.UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordTohash, 13);

                // force current dates to database
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                user.Created = currentDate;
                user.Modified = currentDate;
                    
                // Add user to the database
                db.UserAccounts.Add(user);
                await db.SaveChangesAsync();

                // Log success
                logger.LogInformation("User with ID {UserId} successfully created/ Date/Time: {dateTime}.",user.Id , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                // Return the created user details
                return Results.CreatedAtRoute(GetUserEndpointName, new { id = user.Id }, user.ToUserDetailsDto());
                }
                catch (Exception ex)
                {
                    // Log the exception and failure
                    logger.LogError(ex, "An error occurred while creating a new user/ Date/Time: {dateTime}.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Return an error response
                    return Results.Problem("An error occurred while creating the user");
                }

        }).WithParameterValidation();

        return user;
    }
}

