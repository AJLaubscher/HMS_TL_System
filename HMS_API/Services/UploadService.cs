using System;

namespace HMS_API.Services;

public class UploadService
{
    public async Task<string?> Upload(IFormFile file)
    {
       try
       {
            // Valid extensions
            //List<string> validExtensions = new List<string>() { ".jpg", ".png", ".gif" };
            List<string> validExtensions = new List<string>() { ".mp4", ".mov", ".avi", ".mkv" };
            string extension = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extension))
            {
                return $"Extension is not valid({string.Join(',', validExtensions)})";
            }

            // File size
            long size = file.Length;
            int maxSizeMB = 150;                    // 50mb, 100mb? 150?
            if (size > (maxSizeMB * 1024 * 1024)) // 50mb, 100mb? 150?
            {
                return $"Maximum size can be {maxSizeMB}";
            }

            // Name changing
            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HMS_Assignments", "uploads");

            // Ensure the directory exists
            if (!Directory.Exists(path))
            {
                 Directory.CreateDirectory(path);
            }

            // Using FileStream to save the file asynchronously
            string filePath = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;

       }
       catch(Exception ex)
       {
            // Log the exception message
            Console.WriteLine($"Upload error: {ex.Message}");
            return null;
       } 
    }
}
