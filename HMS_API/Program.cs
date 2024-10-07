using System.Text;
using HMS_API;
using HMS_API.Configurations;
using HMS_API.Data;
using HMS_API.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Connection String setup
var conn = builder.Configuration.GetConnectionString("HMS");
builder.Services.AddSqlServer<HMS_Context>(conn)
                .AddProblemDetails()                            //services for failed requests.
                .AddExceptionHandler<GlobalExceptionHandler>(); //handle unexpected request exceptions

// Configure Kestrel server limits for large uploads
KestrelConfiguration.ConfigureKestrelLimits(builder.WebHost);


builder.Logging.AddJsonConsole(options =>   // logging information in terminal (Json)
{
    options.JsonWriterOptions = new()
    {
        Indented = true
    };
});

// register authorization services
builder.Services.AddAuthorization(options =>{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LecturerPolicy", policy => policy.RequireRole("Lecturer"));
    options.AddPolicy("StudentPolicy", policy => policy.RequireRole("Student"));
});

//configure jwt token services
var configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {  
        var tokenKey = configuration.GetSection("AppSettings:Token").Value;

        if(string.IsNullOrEmpty(tokenKey))
        {
            throw new ArgumentNullException(nameof(tokenKey), "Token key is not provided in the configuration.");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme{
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// setting the name of XSRF token
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

var app = builder.Build();

app.UseRouting();

app.UseAntiforgery();  // Add the Antiforgery middleware

app.UseStatusCodePages();   // default response handler checking responses with status codes between 400 and 599 that do not have a body.
app.UseExceptionHandler();  // catch exceptions, log them, and re-execute the request in an alternate pipeline.

//app.UseHttpLogging();           // log http requests & responses

app.UseAuthentication(); // add authentication & authorization middleware to app
app.UseAuthorization();


// Create an instance of user, module, enrolment, assignment, submisson and feedback endpoints
var userEndpoints = new UserAccountEndpoints(configuration);
var moduleEndpoints = new ModuleEndpoints();
var enrollmentEndpoints = new EnrolmentEndpoints();
var assignmentEndpoints = new AssignmentEndpoints();
var uploadToken = new PostTokenEndpoint();
var submissionEndpoints = new SubmissionEndpoints();
var feedbackEndpoints = new FeedbackEndpoints();

// Call the method to map user, module, enrolment, assignment, submisson and feedback endpoints
userEndpoints.MapUserEndpoints(app);
moduleEndpoints.MapModuleEndpoints(app);
enrollmentEndpoints.MapEnrolmentEndpoints(app);
assignmentEndpoints.MapAssignmentEndpoints(app);
uploadToken.MapPostTokenEndpoint(app);
submissionEndpoints.MapSubmissionEndpoints(app);
feedbackEndpoints.MapFeedbackEndpoints(app);

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}


app.Run();
