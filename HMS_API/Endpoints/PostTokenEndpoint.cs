using System;
using Microsoft.AspNetCore.Antiforgery;

namespace HMS_API.Endpoints;

public class PostTokenEndpoint
{
    public RouteGroupBuilder MapPostTokenEndpoint(WebApplication app)
    {
        var token = app.MapGroup("X-XSRFToken");

            app.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
            {
                var tokens = forgeryService.GetAndStoreTokens(context);
                var xsrfToken = tokens.RequestToken!;
                return TypedResults.Content(xsrfToken, "text/plain");
            });


        return token;
    }
}
