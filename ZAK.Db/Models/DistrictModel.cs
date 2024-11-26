using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace ZAK.Db.Models;

public class DistrictModel
{
    [Key]
    public string name {get;set;} = String.Empty;
    public string color { get; set; } = "White";

    //Address foreign key
    public List<AddressModel> addresses {get; set;} = new List<AddressModel>();

    public DistrictModel() {}

    public DistrictModel(DistrictModel districtModel)
    {
        foreach (PropertyInfo property in typeof(DistrictModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(districtModel, null), null);
        }       
    }
}
