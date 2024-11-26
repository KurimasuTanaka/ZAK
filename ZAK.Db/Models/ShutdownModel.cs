using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Db.Models;

[PrimaryKey (nameof(group), nameof(day), nameof(time), nameof(zone))]
public class ShutdownModel
{
    public int group { get; set; }
    public int day { get; set; }
    public int time { get; set; }
    public int zone { get; set; }

    ShutdownModel() {}
    ShutdownModel(ShutdownModel shutdownModel)
    {
        foreach (PropertyInfo property in typeof(ShutdownModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(shutdownModel, null), null);
        }
    }
}
