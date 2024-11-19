using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorApp.DB.Models;

public class CoefficientModel
{
    [Key]
    public int id { get; set; }
    
    public string parameter { get; set; }
    public string parameterAlias { get; set; }

    public double coefficient { get; set; }
    public bool infinite { get; set; }

    public CoefficientModel() { }
    public CoefficientModel(CoefficientModel applicationModel)
    {
        foreach (PropertyInfo property in typeof(CoefficientModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(applicationModel, null), null);
        }
    }

}
