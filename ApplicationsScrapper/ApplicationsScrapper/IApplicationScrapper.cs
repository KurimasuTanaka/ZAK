
using BlazorApp.DA;

namespace ApplicationsScrappingModule;

    public interface IApplicationScrapper
    {    
        public void ScrapApplicationData(ref List<Application> applications, string applicationsFilePath);

    }
