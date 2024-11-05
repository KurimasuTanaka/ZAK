using System.Reflection;
using BlazorApp.DB;

namespace BlazorApp.DA;

public class Application : ApplicationModel
{
    public double lat = 0;
    public double lon = 0;

    public double priority = 0.0;

    public int daysToDeadline = 0;


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

    public Application() { }
    public Application(ApplicationModel model) : this(model, 0, 0) { }
    public Application(ApplicationModel model, double lat, double lon) : base(model)
    {
        this.lat = lat;
        this.lon = lon;

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

    public void SetupApplicationPriorityLevel(Dictionary<string, double> coefficients, double buildingPriority)
    {
        priority =
            0 /*there should be a distance*/ * coefficients["distance"]
            + buildingPriority * coefficients["housePriority"]
            + daysToDeadline * coefficients["deadline"]
            + (hot ? 1 : 0) * coefficients["urgency"]
            + (statusWasChecked ? 1 : 0) * coefficients["specified"]
            + (freeCable ? 1 : 0) * coefficients["freeCable"]
            - (tarChangeApp ? 1 : 0) * coefficients["tarrifeChangeApplication"];
    }

    public void Copy(Application source)
    {
        foreach (PropertyInfo property in GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(source));
        }
    }
}
