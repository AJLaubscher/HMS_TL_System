using HMS_API;
using HMS_API.Data;
using HMS_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Connection String setup
var conn = builder.Configuration.GetConnectionString("HMS");
builder.Services.AddSqlServer<HMS_Context>(conn)
                .AddProblemDetails()                            //services for failed requests.
                .AddExceptionHandler<GlobalExceptionHandler>(); //handle unexpected request exceptions


builder.Logging.AddJsonConsole(options =>   // JSon format logs
{
    options.JsonWriterOptions = new()
    {
        Indented = true
    };
});

var app = builder.Build();

app.UseStatusCodePages();   // default response handler checking responses with status codes between 400 and 599 that do not have a body.
app.UseExceptionHandler();  // catch exceptions, log them, and re-execute the request in an alternate pipeline.

//app.MapGet("/", () => "Hello World!");

//app.UseHttpLogging();           // log http requests & responses

// Create an instance of user, module, enrolment, assignment, submisson and feedback endpoints
var userEndpoints = new UserAccountEndpoints();
var moduleEndpoints = new ModuleEndpoints();
var enrollmentEndpoints = new EnrolmentEndpoints();
var assignmentEndpoints = new AssignmentEndpoints();
var submissionEndpoints = new SubmissionEndpoints();
var feedbackEndpoints = new FeedbackEndpoints();

// Call the method to map user, module, enrolment, assignment, submisson and feedback endpoints
userEndpoints.MapUserEndpoints(app);
moduleEndpoints.MapModuleEndpoints(app);
enrollmentEndpoints.MapEnrolmentEndpoints(app);
assignmentEndpoints.MapAssignmentEndpoints(app);
submissionEndpoints.MapSubmissionEndpoints(app);
feedbackEndpoints.MapFeedbackEndpoints(app);

app.Run();
