using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace BlazorApp.DB;

public class AddressesCoordinates
{
   // [ForeignKey("addressId")]
    public AddressModel address { get; set; } = new AddressModel();
    [Key]
    public int addressId { get; set; }
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
