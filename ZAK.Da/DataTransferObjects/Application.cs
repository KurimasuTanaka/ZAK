﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BlazorApp.Enums;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class Application : ApplicationModel
{
    public double priority = 0.0;

    public int daysToDeadline = 0;

    public BlackoutZone[] blackoutZones = [BlackoutZone.Unknown, BlackoutZone.Unknown, BlackoutZone.Unknown];

    public string streetName
    {
        get
        {
            return address?.streetName ?? "";
        }
        set
        {
            if (address is not null) address.streetName = value;
        }
    }

    public string building
    {
        get
        {
            return address?.building ?? "";
        }
        set
        {
            if (address is not null) address.building = value;
        }
    }

    public string districtName
    {
        get
        {
            return address.district.name ?? "";
        }
        set
        {
            address.district.name = value;
        }
    }

    public bool applicationWasUpdated
    {
        get
        {
            return addresWasUpdated || masterCommentWasUpdated || operatorCommentWasUpdated || statusWasUpdated;
        }
    }

    public Application() { }
    public Application(ApplicationModel model) : base(model)
    {
        SetupApplicationDeadline();
    }

    public object? this[string propertyName]
    {
        get
        {
            PropertyInfo? myPropInfo = GetType().GetProperty(propertyName);
            if (myPropInfo is not null) return myPropInfo.GetValue(this);
            else
            {
                throw new Exception("Non existing property is used");
            }
        }
        set
        {
            PropertyInfo? myPropInfo = GetType().GetProperty(propertyName);
            if (myPropInfo is not null) myPropInfo.SetValue(this, value, null);
            else
            {
                throw new Exception("Non existing property is used");
            }
        }
    }


    private void SetupApplicationDeadline()
    {
        if (day is 0) day = DateTime.Now.Day;
        if (month is 0) month = DateTime.Now.Month;
        if (year is 0) year = DateTime.Now.Year;

        DateTime dateTime = new DateTime(year, month, day);
        daysToDeadline = (dateTime.AddDays(maxDaysForConnection) - DateTime.Today).Days;

    }

    public void SetupBlackoutZones(List<BlackoutModel> blackoutModels, int day, int time)
    {
        for (int i = 0; i < 3; i++)
        {
            BlackoutZone blackoutZone = blackoutModels.FirstOrDefault(b =>
                b.group == this.address.blackoutGroup &&
                b.day == day && b.time == time + 10 + i)?.zone ?? BlackoutZone.Unknown;

            blackoutZones[i] = blackoutZone;
        }
    }

    public void CalculateApplicationPriorityLevel(Dictionary<string, double> coefficients, List<Application> nearApplications)
    {
        //Return if calculation of distance is impossible 
        if (this.address is null || this.address.coordinates is null)
        {
            priority = 0;
            return;
        }

        //Calculate distance to the nearest applications
        double distance = 1;
        foreach (Application application in nearApplications)
        {
            if (application.address is null || application.address.coordinates is null) break;

            else distance +=
                 (Math.Sqrt(
                     Math.Pow(application.address.coordinates.lat - this.address.coordinates.lat, 2) +
                     Math.Pow(application.address.coordinates.lon - this.address.coordinates.lon, 2)));
        }

        //Calculate priority 
        priority =
            (1 / distance) * coefficients["distance"]
            + (address.addressPriority is null ? 0.0 : address.addressPriority.priority) * coefficients["housePriority"]
            + (urgent ? 1 : 0) * coefficients["urgency"]
            + (statusWasChecked ? 1 : 0) * coefficients["statusCheck"]
            + (freeCable ? 1 : 0) * coefficients["freeCable"]
            - (tarChangeApp ? 1 : 0) * coefficients["tarrifeChangeApplication"];

        if (daysToDeadline > 0) priority += 1 / daysToDeadline * coefficients["deadline"];
        else if (daysToDeadline < 0) priority += Math.Abs(daysToDeadline) / 1 * coefficients["deadline"];
        else priority += 1 * coefficients["deadline"];
    }

    public void Copy(Application source)
    {
        foreach (PropertyInfo property in GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(source));
        }
    }
}

public class ApplicationComparer : IEqualityComparer<Application?>
{
    public bool Equals(Application? x, Application? y)
    {
        if (x is null || y is null) return false;
        if (x.id == y.id) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] Application? obj)
    {
        return obj.id.GetHashCode();
    }
}