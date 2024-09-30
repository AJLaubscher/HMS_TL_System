namespace HMS_TL_System_UnitTests;

using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Diagnostics.CodeAnalysis;

public class InfrastructureTests
{

    private HttpClient GetHttpClient() {
        return  new HttpClient {
            BaseAddress = new System.Uri("http://www.randomnumberapi.com")
        };
    }

    [Test]
    public async Task IsWebRespondingAsync()
    {
        using HttpClient client = new();
        var text = await client.GetStringAsync("http://localhost:5157");

        Assert.That(text, Is.EqualTo("Hello World!"));
        Console.WriteLine(text);
    }

    //Test for GET request
    [Test]
    public async Task getEnpointReturnStatusCode()
    {
        HttpResponseMessage response = await GetHttpClient().GetAsync("http://localhost:5157/");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}

