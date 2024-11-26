using System;
using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class AddressPriority : AddressPriorityModel
{
    public string streetName
    {
        get
        {
            return address.streetName;
        }
        set
        {
            address.streetName = value;
        }
    }

    public string building
    {
        get
        {
            return address.building;
        }
        set
        {
            address.building = value;
        }


    }

    public AddressPriority() { }
    public AddressPriority(AddressPriorityModel model) : base(model) { }

    public void Copy(AddressPriorityModel source)
    {
        foreach (PropertyInfo property in GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(source));
        }
    }
}
