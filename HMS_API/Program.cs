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
builder.Services.AddCors(o => {
    o.AddPolicy(name: "AllowAll", policy => {
        policy.AllowAnyOrigin();
        policy.WithOrigins("http://localhost:5157").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseAntiforgery();  // Add the Antiforgery middleware

app.UseStatusCodePages();   // default response handler checking responses with status codes between 400 and 599 that do not have a body.
app.UseExceptionHandler();  // catch exceptions, log them, and re-execute the request in an alternate pipeline.

//app.UseHttpLogging();           // log http requests & responses

// Add authentication & authorization middleware to app
app.UseAuthentication(); 
app.UseAuthorization();

// Create an instances and map endpoints
var userEndpoints = new UserAccountEndpoints(configuration); 
userEndpoints.MapUserEndpoints(app);

var moduleEndpoints = new ModuleEndpoints();
moduleEndpoints.MapModuleEndpoints(app);

var enrollmentEndpoints = new EnrolmentEndpoints();
enrollmentEndpoints.MapEnrolmentEndpoints(app);

var assignmentEndpoints = new AssignmentEndpoints();
assignmentEndpoints.MapAssignmentEndpoints(app);

var uploadToken = new PostTokenEndpoint();
uploadToken.MapPostTokenEndpoint(app);

var submissionEndpoints = new SubmissionEndpoints();
submissionEndpoints.MapSubmissionEndpoints(app);

var feedbackEndpoints = new FeedbackEndpoints();
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
