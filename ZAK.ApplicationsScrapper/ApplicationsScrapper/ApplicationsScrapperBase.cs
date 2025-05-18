using System;
using ApplicationsScrappingModule;
using ZAK.DA;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ApplicationsScrappingModule;

public abstract class ApplicationsScrapperBase : IApplicationsScrapper
{
    protected HtmlDocument _applicationsDoc  = new HtmlDocument();

    protected ILogger<ApplicationsScrapperBase> _logger;

    public ApplicationsScrapperBase(ILogger<ApplicationsScrapperBase> logger)
    {
        _logger = logger;
    }

    private async Task LoadApplicationsFile(string applicationsFilePath)
    {
        _logger.LogInformation($"Loading applications file from {applicationsFilePath}...");
        
        StreamReader streamReader = new StreamReader(applicationsFilePath);
        string fileData = await streamReader.ReadToEndAsync();
        streamReader.Close();

        _logger.LogInformation("Applications file loaded successfully!");

        _logger.LogInformation("Loadign html file...");

        _applicationsDoc.LoadHtml(fileData);

        _logger.LogInformation("HTML file was loaded successfully!");
    }

    protected abstract Task<List<Application>> ProceedApplicationsScrapping();

    public async Task<List<Application>> ScrapApplicationData(string applicationsFilePath)
    {
        await LoadApplicationsFile(applicationsFilePath);
        return await ProceedApplicationsScrapping();
    }
}
