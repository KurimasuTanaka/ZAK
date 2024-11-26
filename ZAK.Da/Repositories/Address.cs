using System;
using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class Address : AddressModel
{
    public double priority
    {
        get
        {
            return addressPriority is null ? 0.0 : addressPriority.priority;
        }
        set
        {
            if (addressPriority is null)
            {
                addressPriority = new AddressPriorityModel();
                addressPriority.priority = value;
            }
            else addressPriority.priority = value;
        }
    }

    public string streetAlias 
    {
        get 
        {
            return addressAlias is null ? "" : addressAlias.streetAlias;
        }
        set
        {
            if (addressAlias is null)
            {
                addressAlias = new AddressAliasModel();
                addressAlias.streetAlias = value;
            }
            else addressAlias.streetAlias = value;
        }
    }

    public string buildingAlias 
    {
        get 
        {
            return addressAlias is null ? "" : addressAlias.buildingAlias;
        }
        set
        {
            if (addressAlias is null)
            {
                addressAlias = new AddressAliasModel();
                addressAlias.buildingAlias = value;
            }
            else addressAlias.buildingAlias = value;
        }
    }


    public double Latitude 
    {
        get
        {
            return coordinates is null ? 0.0 : coordinates.lat;
        }
        set
        {
            if (coordinates is null)
            {
                coordinates = new AddressCoordinatesModel();
                coordinates.lat = value;
            }
            else coordinates.lat = value;
        }
    }

    public double Longtitude 
    {
        get
        {
            return coordinates is null ? 0.0 : coordinates.lon;
        }
        set
        {
            if (coordinates is null)
            {
                coordinates = new AddressCoordinatesModel();
                coordinates.lon = value;
            }
            else coordinates.lon = value;
        }
    }


    public bool resolved = false;
    public bool connected = false;

    public Address() : base()
    {
    }

    public Address(AddressModel data) : base(data)
    {
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

}
