﻿using System.Reflection;
using BlazorApp.DB;

namespace BlazorApp.DA;

public class Brigade : BrigadeModel
{
    public List<Application> applications = new List<Application>(new Application[9]);

    public Brigade() { }
    public Brigade(BrigadeModel model) : base(model) {}

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


    public void PopulateApplicationList(IApplicationsDataAccess applicationsDataAccess)
    {
        for(int i = 0; i < 9; i++)
        {
            if(applicationsIds[i] == 0) 
            {
                applications[i] = null;
                continue;
            }
            else applications[i] = applicationsDataAccess.GetApplication(applicationsIds[i]).Result;
        }
    }
}
