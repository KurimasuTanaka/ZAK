using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp;

public class FileLoader : IFileLoader
{

    ILogger<FileLoader> _logger;
    public readonly IWebHostEnvironment _enviroment;

    List<string> tempFiles = new List<string>();
    
    public FileLoader(IWebHostEnvironment enviroment, ILogger<FileLoader> logger)
    {
        _enviroment = enviroment;
        _logger = logger;
    }

    public FileLoader()
    {
    }

    public void DeleteTempFiles()
    {    
        foreach(string tempFile in tempFiles)
        {
            File.Delete(tempFile);
        }
    }

    public async Task LoadFile(IBrowserFile file)
    {
        _logger.LogInformation($"Loading file {file.Name}...");

        if(file is not null)
        {
            tempFiles.Add($"{_enviroment.ContentRootPath}/{file.Name}TEMP.html");
            
            using (FileStream fs = new FileStream(tempFiles.Last(), FileMode.Create))
            {
                await file.OpenReadStream(file.Size).CopyToAsync(fs);
                fs.Close();
            }
        } else throw new ArgumentNullException("File in file loader is null!!!");
    
        _logger.LogInformation("File loaded successfully!");
    }

    public string GetLastLoadedFile()
    {
        return tempFiles.Last();
    }
}
