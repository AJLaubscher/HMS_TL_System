using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HMS_API.Data;
using HMS_API.Dtos.userAccount;
using HMS_API.Entities;
using HMS_API.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HMS_API.Endpoints;

public class UserAccountEndpoints
{

    private readonly ILogger<UserAccountEndpoints> logger;
    private readonly IConfiguration _configuration;
     
     // Constructor for Dependency Injection
    public UserAccountEndpoints(IConfiguration configuration)
    {
        _configuration = configuration;
        
        logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<UserAccountEndpoints>();
    }

    const string GetUserEndpointName = "GetUser";

    public RouteGroupBuilder MapUserEndpoints(WebApplication app)
    {
        var user = app.MapGroup("users").WithParameterValidation()
        .RequireAuthorization();


        // Request all users
        user.MapGet("/", async (HMS_Context db, ClaimsPrincipal account) => {
            try
            {
                if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                    return Results.Forbid(); // Return 403 Forbidden if neither role
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
        }).RequireAuthorization();


        // Get a specific user by ID
        user.MapGet("/{id}", async (int id, HMS_Context db, ClaimsPrincipal account) => 
        {
            if(VerifyAdminLecturerClaim(account)== false) // Check if the user has the Admin or Lecturer role using HasClaim
                return Results.Forbid(); // Return 403 Forbidden if neither role

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

        }).RequireAuthorization().WithName(GetUserEndpointName);



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
        }).WithParameterValidation().RequireAuthorization("AdminPolicy");

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
        }).WithParameterValidation().RequireAuthorization("AdminPolicy");


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
        }).RequireAuthorization("AdminPolicy");

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

        }).WithParameterValidation().RequireAuthorization("AdminPolicy");

        // user log in
        user.MapPost("/login", (LoginDto login, HMS_Context db) => 
        {

            // Bcrypt method, if works, no use to change db
            var user = db.UserAccounts.FirstOrDefault(x => x.Username == login.Username);

            // not found    
            if(user == null)
                return Results.Unauthorized();

            if(!VerifyPassword(login.UserPassword, user.UserPassword))
            {
                logger.LogWarning("Password verification failed for user {Username}", login.Username);
                return Results.BadRequest("Incorrect credentials");
            }

            // Generate JWT token and return on success
            string token = CreateToken(user);
            return Results.Ok(new{Token = token});
                
        }).AllowAnonymous();

        return user;
    }

    private bool VerifyPassword(string password, string storedPassword)
    {
        // return true/false
        bool verify = BCrypt.Net.BCrypt.EnhancedVerify(password, storedPassword);
        return verify;
    }

    private string CreateToken(UserAccount user)
    {
        string role = ""; 

        if(user.UserRole <= 3 && user.UserRole >= 1)
        { 
            if( user.UserRole == 1)
            {
                role = "Admin";
            }
            else if (user.UserRole == 2)
            {
                role = "Lecturer";
            }
            else
            {
                role = "Student";
            }
        }
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, role)
        };

        var tokenKey = _configuration.GetValue<string>("AppSettings:Token"); // get token from appsettings

        if(string.IsNullOrEmpty(tokenKey)) 
        {
            throw new ArgumentException("AppSettings:Token", "JWT token secret not found in configuration.");
        }

        // Log the key length (this is for debugging purposes only, don't log keys in production!)
        logger.LogInformation("JWT Token Key Length: {KeyLength}", tokenKey.Length);

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));

        // Check if the key is of sufficient length
        if (key.KeySize < 512) // Key size is in bits
        {
            throw new ArgumentOutOfRangeException(nameof(tokenKey), "JWT token key size must be at least 512 bits.");
        }

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred
        ); 

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
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

