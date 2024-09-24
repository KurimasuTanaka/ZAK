using System;
using ApplicationsScrappingModule;
using BlazorApp.DA;
using HtmlAgilityPack;

namespace ApplicationsScrappingModule;

public abstract class ApplicationsScrapperBase : IApplicationsScrapper
{
    protected HtmlDocument _applicationsDoc  = new HtmlDocument();

    private async Task LoadApplicationsFile(string applicationsFilePath)
    {
        StreamReader streamReader = new StreamReader(applicationsFilePath);
        string fileData = await streamReader.ReadToEndAsync();
        streamReader.Close();

        _applicationsDoc.LoadHtml(fileData);
    }

    protected abstract Task<List<Application>> ProceedApplicationsScrapping();

    public async Task<List<Application>> ScrapApplicationData(string applicationsFilePath)
    {
        await LoadApplicationsFile(applicationsFilePath);
        return await ProceedApplicationsScrapping();
    }
}
