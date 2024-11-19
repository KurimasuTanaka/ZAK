using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp;

public interface IFileLoader
{
    public Task LoadFile(IBrowserFile file);

    public void DeleteTempFiles();

    public string GetLastLoadedFile();
}
