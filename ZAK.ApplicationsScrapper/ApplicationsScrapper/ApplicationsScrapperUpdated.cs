using System;
using ZAK.DA;
using BlazorApp.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ApplicationsScrappingModule;

public class ApplicationsScrapperUpdated : ApplicationsScrapperBase
{
    public ApplicationsScrapperUpdated(ILogger<ApplicationsScrapperBase> logger) : base(logger)
    {
    }

    protected override Task<List<Application>> ProceedApplicationsScrapping()
    {
        _logger.LogInformation("Proceeding applications scrapping...");


        List<Application> applications = new List<Application>();
        HtmlNodeCollection applicationNodes = _applicationsDoc.DocumentNode.SelectNodes("/html/body/div[7]/div[2]/table/tbody/tr");

        if (applicationNodes != null)
        {
            foreach (HtmlNode applicationNode in applicationNodes)
            {
                applications.Add(ProceedApplicationNode(applicationNode));
            }
        }

        _logger.LogInformation("Applications scrapping was successful!");

        return Task.FromResult(applications);
    }


    private Application ProceedApplicationNode(HtmlNode applicationNode)
    {
        Application application = new Application();
        application.address = new Address();
        application.address.district = new District();

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

        application = TryScrapApplicationDeadline(application);
        application = TryScrapApplicationArrangmentStatus(application);
        application = TryScrapAddressBlackoutGroup(application);

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

        application.address!.district!.name = applicationNode.SelectSingleNode("td[2]/b").InnerHtml;
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
        if (
            application.operatorComment.Contains("вільний кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("свободный кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("має бути кабель", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("должен быть кабель", StringComparison.CurrentCultureIgnoreCase)
        ) application.freeCable = true;

        return application;
    }
    private Application TryScrapApplicationStatusWasChecked(Application application, HtmlNode applicationNode)
    {
        if (
            application.operatorComment.Contains("цікавився статусом", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("интересовался статусом", StringComparison.CurrentCultureIgnoreCase)
        ) application.statusWasChecked = true;

        return application;
    }
    private Application TryScrapApplicationTarChangeApp(Application application, HtmlNode applicationNode)
    {
        if (
            application.operatorComment.Contains("Перехід на Гігабіт", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("перехід", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("переход", StringComparison.CurrentCultureIgnoreCase)
        ) application.tarChangeApp = true;


        return application;
    }
    private Application TryScrapApplicationTiming(Application application, HtmlNode applicationNode)
    {
        //Part of the day
        if (
            application.operatorComment.Contains("першу половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("1 половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("первую половину", StringComparison.CurrentCultureIgnoreCase)
        )
        {
            application.firstPart = true;
            return application;
        }

        if (
            application.operatorComment.Contains("другу половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("вторую половину", StringComparison.CurrentCultureIgnoreCase) ||
            application.operatorComment.Contains("2 половину", StringComparison.CurrentCultureIgnoreCase)
        )
        {
            application.secondPart = true;
            return application;
        }

        return application;
    }

    private Application TryScrapApplicationDeadline(Application application)
    {
        string timerangeLine = "";
        try
        {
            int index = application.operatorComment.IndexOf("Терміни");
            if(index == -1) return application;
            timerangeLine = application.operatorComment.Substring(index);
        }
        catch (Exception e)
        {
            return application;
        }

        string[] timerange = timerangeLine.Split([' ', '-']);

        int number = 0;
        bool firstNumberWasFound = false;
        bool vidWasFound = false;
        foreach (string word in timerange)
        {
            //In case terms are written in "Від 10 днів" format
            if (vidWasFound)
            {
                string wordWithoutNumber = new string(word.Where(c => char.IsDigit(c)).ToArray());

                Int32.TryParse(wordWithoutNumber, out number);
                if (number != 0)
                {
                    application.maxDaysForConnection = number + 10;
                    return application;
                }

            }
            if (word == "від")
            {
                vidWasFound = true;
                continue;
            }

            //In regular case
            Int32.TryParse(word, out number);
            if (number != 0)
            {
                if (firstNumberWasFound)
                {
                    application.maxDaysForConnection = number;
                }
                else firstNumberWasFound = true;
            }
        }

        return application;
    }

    private Application TryScrapApplicationArrangmentStatus(Application application)
    {
        if (
            application.operatorComment.ToLower().Contains("домовлена") ||
            application.operatorComment.ToLower().Contains("договорена") ||
            application.operatorComment.ToLower().Contains("домовлено") ||
            application.operatorComment.ToLower().Contains("договорено")
        ) application.ignored = true;

        return application;
    }

    private Application TryScrapAddressBlackoutGroup(Application application)
    {
        int groupLineIndex = application.operatorComment.ToLower().IndexOf("група");
        if (groupLineIndex == -1) return application;

        int length = 15;
        if (groupLineIndex + length > application.operatorComment.Length)
        {
            length = application.operatorComment.Length - groupLineIndex;
        }

        string groupLine = application.operatorComment.Substring(groupLineIndex, length);
        string[] words = groupLine.Split(' ');
        foreach (string word in words)
        {
            string wordToParse = new string(word.Where(c => char.IsDigit(c)).ToArray());
            if (wordToParse.Length == 0) continue;

            if (Int32.TryParse(wordToParse, out int group))
            {
                application.address.blackoutGroup = group;
                return application;
            }
        }

        return application;
    }
}
