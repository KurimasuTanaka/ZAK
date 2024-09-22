using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorApp.DB;

public class AddressesCoordinates
{
    [Key]
    public string address { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }

    public AddressesCoordinates() {}
    public AddressesCoordinates(AddressModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressesCoordinates).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }

}
