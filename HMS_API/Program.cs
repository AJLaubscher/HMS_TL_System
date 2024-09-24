using HMS_API;
using HMS_API.Data;
using HMS_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Connection String setup
var conn = builder.Configuration.GetConnectionString("HMS");
builder.Services.AddSqlServer<HMS_Context>(conn)
                .AddProblemDetails()                            //services for failed requests.
                .AddExceptionHandler<GlobalExceptionHandler>(); //handle unexpected request exceptions

var app = builder.Build();

app.UseStatusCodePages();   // default response handler checking responses with status codes between 400 and 599 that do not have a body.
app.UseExceptionHandler();  // catch exceptions, log them, and re-execute the request in an alternate pipeline.


//app.MapGet("/", () => "Hello World!");

app.MapUserEndpoints();
app.MapModuleEndpoints();
app.MapEnrolmentEndpoints();
app.MapAssignmentEndpoints();
app.MapSubmissionEndpoints();
app.MapFeedbackEndpoints();

app.Run();
