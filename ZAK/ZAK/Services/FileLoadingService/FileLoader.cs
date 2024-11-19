using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp;

public class FileLoader : IFileLoader
{
    List<string> tempFiles = new List<string>();
    public readonly IWebHostEnvironment _enviroment;
    public FileLoader(IWebHostEnvironment enviroment)
    {
        _enviroment = enviroment;
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
        //TODO: Add a string builder

        if(file != null)
        {
            tempFiles.Add(_enviroment.ContentRootPath + "/" + file.Name + "TEMP" + ".html");
            
            using (FileStream fs = new FileStream(tempFiles.Last(), FileMode.Create))
            {
                await file.OpenReadStream(file.Size).CopyToAsync(fs);
                fs.Close();
            }
        }
    }

    public string GetLastLoadedFile()
    {
        return tempFiles.Last();
    }
}
