using System;
using System.Reflection;
using BlazorApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Db.Models;

[PrimaryKey (nameof(group), nameof(day), nameof(time))]
public class BlackoutModel
{
    public int group { get; set; }
    public int day { get; set; }
    public int time { get; set; }
    public BlackoutZone zone { get; set; }

    public BlackoutModel() {}
    public BlackoutModel(BlackoutModel blackoutModel)
    {
        foreach (PropertyInfo property in typeof(BlackoutModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(blackoutModel, null), null);
        }
    }
}
