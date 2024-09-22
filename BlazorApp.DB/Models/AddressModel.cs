using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorApp.DB;

public enum District
{

}

public class AddressModel 
{
    [Key]
    public int id {get; set;}
    public string streetName  {get; set;} = ""; 
    public string building {get; set;}

    public double priority { get; set; } = 0.0;

    public AddressModel() {}
    public AddressModel(AddressModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }
}