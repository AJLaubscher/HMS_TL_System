using System;
using Microsoft.EntityFrameworkCore;

namespace HMS_API.Data;

public static class DataExtentions
{
    public static async Task MigrateDBAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HMS_Context>();
        await db.Database.MigrateAsync();
    }
}
