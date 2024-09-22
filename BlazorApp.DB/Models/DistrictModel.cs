using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace BlazorApp.DB;

public class DistrictModel
{
    [Key]
    public string name {get;set;} = "";
    public string color { get; set; } = "White";

    public DistrictModel() {}

    public DistrictModel(DistrictModel districtModel)
    {
        foreach (PropertyInfo property in typeof(DistrictModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(districtModel, null), null);
        }       
    }
}
