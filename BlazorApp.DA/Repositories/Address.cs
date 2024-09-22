using System;
using System.Reflection;
using BlazorApp.DB;

namespace BlazorApp.DA;

public class Address: AddressModel
{
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
