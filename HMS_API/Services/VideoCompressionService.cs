using System;
using Xabe.FFmpeg;

namespace HMS_API.Services;

public class VideoCompressionService
{
    public VideoCompressionService()
    {
        // Set the path to the directory where FFmpeg executables are located
        FFmpeg.SetExecutablesPath(Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg"));
    }
    public async Task<string?> CompressVideo(string videoFilePath, string outputDirectory)
    {
        if (string.IsNullOrEmpty(outputDirectory))
        {
            Console.WriteLine("Invalid directory path.");
            return null;
        }

        // Ensure the directory exists
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        string compressedFilePath = Path.Combine(outputDirectory, Path.GetFileName(videoFilePath));

        try
        {
            // FFmpeg compression using Xabe.FFmpeg
            var conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(videoFilePath, compressedFilePath);

            var result = await conversion.Start();

            if (File.Exists(compressedFilePath))
            {
                return compressedFilePath;
            }
            else
            {
                return null; // Compression failed
            }        
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Compression failed: {ex.Message}");
            return null;
        }
    }
}
