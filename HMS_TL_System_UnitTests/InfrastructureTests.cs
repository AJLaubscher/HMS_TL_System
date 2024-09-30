namespace HMS_TL_System_UnitTests;

using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework.Internal;
using Azure;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Text.Json;
using HMS_API.Dtos.assignment;

[TestFixture]
public class InfrastructureTests
{

    private HttpClient GetHttpClient() {
        return  new HttpClient {
            BaseAddress = new System.Uri("http://localhost:5157")
        };
    }

    //Test assignment
    [Test]
    public async Task getAssignmentEnptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("assignments");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //Test Enrollment
    [Test]
    public async Task getEnrollmentEndptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("enrolments");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    //Test Enrollment
    [Test]
    public async Task getFeedbackEndptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("feedback");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task getModuleEndptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("modules");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task getSubmissionEndptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("submissions");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

        [Test]
    public async Task getUserEndptReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("users");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    // [Test]
    public async Task getAssignmentEdpt_ReturnsCorrectData()
    {
        // HttpResponseMessage response = await GetHttpClient().GetAsync("http://localhost:5157/assignments/1");
        // response.EnsureSuccessStatusCode();
        // var content = await response.Content.ReadAsStringAsync();
    }


    [Test]
    public async Task currentlyErrorPost() {
        var requestDto = new CreateAssignmentDto(2, "Mr Bean", "Instructionss", DateOnly.Parse("2024-10-01"), DateOnly.Parse("2024-10-05"), 50, "");

        var response = await GetHttpClient().PostAsJsonAsync<CreateAssignmentDto>(
            "assignments", 
            requestDto
        );

        switch (response.StatusCode) {
            case HttpStatusCode.InternalServerError: 
                var json = response.Content.ReadAsStream();
                Console.WriteLine(json);
                Console.WriteLine("---------------------");
                var error = JsonSerializer.Deserialize<InternalServerError>(json!);
                Console.WriteLine(error!.ToString());

                var serialized = JsonSerializer.Serialize(error);
                Console.WriteLine(serialized);
                break;

            default:
                break;
        }

    }


    public class InternalServerError {


        [JsonPropertyName("type")]
        public string Type {get; set;}

        [JsonPropertyName("title")]
        public string Title { get; set;}

        [JsonPropertyName("status")]
        public int Status { get; set;}

        [JsonPropertyName("detail")]
        public string Detail {get ;set;}

        public string ToString() {
            return $"{Type}; {Title}; mapped with error {Status} --- {Detail}";
        }
    }


}

