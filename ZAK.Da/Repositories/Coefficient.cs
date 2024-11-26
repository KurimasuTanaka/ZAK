using System;
using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class Coefficient : CoefficientModel
{
    public Coefficient() : base()
    {
    }

    public Coefficient(CoefficientModel data) : base(data)
    {
    }

    
    public Coefficient(double newCoefficient)
    {
        this.coefficient = newCoefficient;
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
