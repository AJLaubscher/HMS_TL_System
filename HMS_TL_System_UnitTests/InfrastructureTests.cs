namespace HMS_TL_System_UnitTests;

using NUnit.Framework;

public class InfrastructureTests
{
    [Test]
    public async Task IsWebRespondingAsync()
    {
        using HttpClient client = new();
        var text = await client.GetStringAsync("http://localhost:5157");

        Assert.That(text, Is.EqualTo("Hello World!"));
        Console.WriteLine(text);
    }
}
