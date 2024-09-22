using HtmlAgilityPack;

using BlazorApp.DA;
using BlazorApp.Enums;

namespace ApplicationsScrappingModule;

public class ApplicationScrapper : IApplicationScrapper
{
    IApplicationsDataAccess _databaseModule;
    IDistrictDataAccess _districtDataAccess;
    public ApplicationScrapper(IApplicationsDataAccess databaseModule, IDistrictDataAccess districtDataAccess)
    {
        _databaseModule = databaseModule;
        _districtDataAccess = districtDataAccess;
    }

    public List<Application> GetApplicationsListFromNodeCollection(HtmlNodeCollection nodeCollection)
    {
        List<Application> applications = new List<Application>();

        try
        {
            for (int application = 1; application < nodeCollection.Count - 3; application += 3)
            {
                applications.Add(GetAppModelFromNode(
                    nodeCollection[application + 1],
                    nodeCollection[application + 2]));
            }
        }
        catch
        {
            applications.Clear();
            Application application = new Application();

            application.id = 0;
            application.operatorComment = "ERROR DATA";
            applications.Add(application);
            return applications;
        }


        return applications;

        Application GetAppModelFromNode(HtmlNode dataNode1, HtmlNode dataNode2)
        {
            Application application = new Application();

            application.id = Int32.Parse(dataNode1.SelectSingleNode("./td[1]/b/a").InnerHtml);

            //Scrapping district
            application.districtName = dataNode1.SelectSingleNode("./td[2]/b/font").InnerHtml;

            application.streetName = dataNode1.SelectSingleNode("./td[3]/b/font").InnerHtml;
            application.building = dataNode1.SelectSingleNode("./td[4]/b/font").InnerHtml;

            //Stretching status scrapping

            //TODO: Resolve this shit
            try
            {
                HtmlAttributeCollection stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[2].Attributes;

                application.stretchingStatus = StretchingStatus.NotSctreched;


                stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[2].Attributes;
                if (stretchingOptions.Count == 3)
                {
                    application.stretchingStatus = StretchingStatus.Streched;
                }

                stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[3].Attributes;
                if (stretchingOptions.Count == 3)
                {
                    application.stretchingStatus = StretchingStatus.DoNotStrech;
                }
            }
            catch
            {

            }

            application.operatorComment = dataNode1.SelectSingleNode("./td[8]").InnerHtml.Substring(58);
            application.masterComment = dataNode2.SelectSingleNode("./td[4]").InnerHtml;

            //Date scrapping
            application.day = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(6, 2));
            application.month = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(9, 2));
            application.year = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(12, 4));

            //Setting up parameters that should be set up by operator
            application.startHour = 10;
            application.endHour = 19;
            application.statusWasChecked = false;
            application.tarChangeApp = false;

            return application;
        }

    }

    public void GetApplicationsListFromNodeCollection(ref List<Application> newApplications, HtmlNodeCollection nodeCollection)
    {
        try
        {
            for (int application = 1; application < nodeCollection.Count - 3; application += 3)
            {
                newApplications.Add(GetAppModelFromNode(
                    nodeCollection[application + 1],
                    nodeCollection[application + 2]));
            }
        }
        catch
        {
            newApplications.Clear();
            Application application = new Application();

            application.id = 0;
            application.operatorComment = "ERROR DATA";
            newApplications.Add(application);
        }

        Application GetAppModelFromNode(HtmlNode dataNode1, HtmlNode dataNode2)
        {
            Application application = new Application();

            application.id = Int32.Parse(dataNode1.SelectSingleNode("./td[1]/b/a").InnerHtml);

            //Scrapping district
            application.districtName = dataNode1.SelectSingleNode("./td[2]/b/font").InnerHtml;

            application.streetName = dataNode1.SelectSingleNode("./td[3]/b/font").InnerHtml;
            application.building = dataNode1.SelectSingleNode("./td[4]/b/font").InnerHtml;

            //Stretching status scrapping

            //TODO: Resolve this shit
            try
            {
                HtmlAttributeCollection stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[2].Attributes;

                application.stretchingStatus = StretchingStatus.NotSctreched;


                stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[2].Attributes;
                if (stretchingOptions.Count == 3)
                {
                    application.stretchingStatus = StretchingStatus.Streched;
                }

                stretchingOptions = dataNode1.SelectNodes("./td[7]/font/select/option")[3].Attributes;
                if (stretchingOptions.Count == 3)
                {
                    application.stretchingStatus = StretchingStatus.DoNotStrech;
                }
            }
            catch
            {

            }

            application.operatorComment = dataNode1.SelectSingleNode("./td[8]").InnerHtml.Substring(58);
            application.masterComment = dataNode2.SelectSingleNode("./td[4]").InnerHtml;

            //Date scrapping
            application.day = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(6, 2));
            application.month = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(9, 2));
            application.year = Int32.Parse(dataNode2.SelectSingleNode("./td[5]").InnerHtml.Substring(12, 4));

            //Setting up parameters that should be set up by operator
            application.startHour = 10;
            application.endHour = 19;
            application.statusWasChecked = false;
            application.tarChangeApp = false;

            return application;
        }
    }


    public void ScrapApplicationData(ref List<Application> applications, string applicationsFilePath)
    {
        StreamReader streamReader = new StreamReader(applicationsFilePath);
        string fileData = streamReader.ReadToEnd();
        streamReader.Close();

        HtmlDocument applicationsDoc = new HtmlDocument();
        applicationsDoc.LoadHtml(fileData);

        HtmlNodeCollection applicationsNodes = applicationsDoc.DocumentNode.SelectNodes("//table[contains(@class,'samplelist')]/tbody/tr");

        GetApplicationsListFromNodeCollection(ref applications, applicationsNodes);
    }
}