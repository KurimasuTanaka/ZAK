
using BlazorApp.DA;

namespace ApplicationsScrappingModule;

    public interface IApplicationsScrapper
    {    
        public Task<List<Application>> ScrapApplicationData(string applicationsFilePath);

    }
