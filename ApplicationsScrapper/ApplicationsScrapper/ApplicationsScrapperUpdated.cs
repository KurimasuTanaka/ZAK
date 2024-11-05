using System;
using BlazorApp.DA;
using BlazorApp.Enums;
using HtmlAgilityPack;

namespace ApplicationsScrappingModule;

public class ApplicationsScrapperUpdated : ApplicationsScrapperBase
{
    protected override Task<List<Application>> ProceedApplicationsScrapping()
    {
        List<Application> applications = new List<Application>();
        HtmlNodeCollection applicationNodes = _applicationsDoc.DocumentNode.SelectNodes("/html/body/div[5]/div[2]/table/tbody/tr");

        foreach (HtmlNode applicationNode in applicationNodes)
        {
            applications.Add(ProceedApplicationNode(applicationNode));
        }

        return Task.FromResult(applications);
    }


    private Application ProceedApplicationNode(HtmlNode applicationNode)
    {
        Application application = new Application();

        application = ScrapApplicationId(application, applicationNode);

        application = ScrapApplicationDistrict(application, applicationNode);
        
        application = ScrapApplicationStreetName(application, applicationNode);
        application = ScrapApplicationBuilding(application, applicationNode);
        
        application = ScrapApplicationDate(application, applicationNode);
        application = ScrapApplicationStretchingStatus(application, applicationNode);
        application = ScrapApplicationOperatorComment(application, applicationNode);
        application = ScrapApplicationMasterComment(application, applicationNode);
    
        application = TryScrapApplicationFreeCable(application, applicationNode);
        application = TryScrapApplicationStatusWasChecked(application, applicationNode);
        application = TryScrapApplicationTarChangeApp(application, applicationNode);
        application = TryScrapApplicationTiming(application, applicationNode);


        return application;
    }

    private Application ScrapApplicationId(Application application, HtmlNode applicationNode)
    {
        application.id = Int32.Parse(applicationNode.SelectSingleNode("td[1]/p[1]/a/span").InnerHtml);
        return application;
    }
    private Application ScrapApplicationDistrict(Application application, HtmlNode applicationNode)
    {
        //application.address.district.name = applicationNode.SelectSingleNode("td[2]/b").InnerHtml;
        application.districtName = applicationNode.SelectSingleNode("td[2]/b").InnerHtml;
        return application;
    }
    private Application ScrapApplicationStreetName(Application application, HtmlNode applicationNode)
    {
        application.streetName = applicationNode.SelectSingleNode("td[3]/b").InnerHtml;
        return application;
    }
    private Application ScrapApplicationBuilding(Application application, HtmlNode applicationNode)
    {
        application.building = applicationNode.SelectSingleNode("td[4]/b").InnerHtml;
        return application;
    }
    private Application ScrapApplicationDate(Application application, HtmlNode applicationNode)
    {
        string date = applicationNode.SelectSingleNode("td[9]/span[4]").InnerHtml;
        application.day = Int32.Parse(date.Substring(6, 2));
        application.month = Int32.Parse(date.Substring(9, 2));
        application.year = Int32.Parse(date.Substring(12, 4));
        return application;
    }
    private Application ScrapApplicationStretchingStatus(Application application, HtmlNode applicationNode)
    {
        HtmlAttribute backgroundColor = applicationNode.SelectNodes("td[7]")[0].Attributes[1];


        switch (backgroundColor.Value)
        {
            case "background-color: rgb(173, 216, 230)":
                application.stretchingStatus = StretchingStatus.DoNotStrech;
                break;
            case "background-color: rgb(144, 238, 144);":
                application.stretchingStatus = StretchingStatus.Streched;
                break;
            default:
                application.stretchingStatus = StretchingStatus.NotSctreched;
                break;
        }


        return application;
    }
    private Application ScrapApplicationOperatorComment(Application application, HtmlNode applicationNode)
    {
        application.operatorComment = applicationNode.SelectSingleNode("td[8]/span[1]").InnerHtml;
        return application;
    }
    private Application ScrapApplicationMasterComment(Application application, HtmlNode applicationNode)
    {
        application.masterComment = applicationNode.SelectSingleNode("td[8]/span[2]").InnerHtml;

        return application;
    }


    //Attempt to scrap the information from the master comment

    private Application TryScrapApplicationFreeCable(Application application, HtmlNode applicationNode)
    {
        if(
            application.operatorComment.Contains("вільний кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("свободный кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("має бути кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("должен быть кабель", StringComparison.CurrentCultureIgnoreCase) 
        ) application.freeCable = true;
        
        return application;
    }
    private Application TryScrapApplicationStatusWasChecked(Application application, HtmlNode applicationNode)
    {
        if(
            application.operatorComment.Contains("цікавився статусом", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("интересовался статусом", StringComparison.CurrentCultureIgnoreCase) 
        ) application.statusWasChecked = true;

        return application;
    }
    private Application TryScrapApplicationTarChangeApp(Application application, HtmlNode applicationNode)
    {
        if(
            application.operatorComment.Contains("Перехід на Гігабіт", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("перехід", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("переход", StringComparison.CurrentCultureIgnoreCase) 
        ) application.tarChangeApp= true;


        return application;
    }
    private Application TryScrapApplicationTiming(Application application, HtmlNode applicationNode)
    {
        if(
            application.operatorComment.Contains("першу половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("1 половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("первую половину", StringComparison.CurrentCultureIgnoreCase) 
        ) application.firstPart = true;

        if(
            application.operatorComment.Contains("другу половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("вторую половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("2 половину", StringComparison.CurrentCultureIgnoreCase) 
        ) application.secondPart = true;

        return application;
    }
}
