using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ZAK.Db.Models;

public class CoefficientModel
{
    [Key]
    public int id { get; set; }
    
    public string parameter { get; set; }
    public string parameterAlias { get; set; } = String.Empty;

    public double coefficient { get; set; } = 1;
    public bool infinite { get; set; } = false;

    public CoefficientModel() { }
    public CoefficientModel(CoefficientModel coefficientModel)
    {
        foreach (PropertyInfo property in typeof(CoefficientModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(coefficientModel, null), null);
        }
    }

}
