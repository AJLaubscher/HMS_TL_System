using System;

namespace HMS_API.Configurations;

public class KestrelConfiguration
{
    public static void ConfigureKestrelLimits(IWebHostBuilder builder)
    {
        builder.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 104857600; // Set to 100 MB (adjust as needed)
        });
    }
}
