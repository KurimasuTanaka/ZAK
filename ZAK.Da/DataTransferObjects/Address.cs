using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class Address : AddressModel
{


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

public class AddressComparer : IEqualityComparer<Address?>
{
    public bool Equals(Address? x, Address? y)
    {
        if (x is null || y is null) return false;
        if (x.streetName == y.streetName && x.building == y.building) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] Address? obj)
    {
        return (obj.streetName + obj.building).GetHashCode();
    }
}
