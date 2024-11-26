using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ZAK.Db.Models;

public class AddressCoordinatesModel
{
   // [ForeignKey("addressId")]
    public AddressModel address { get; set; } = new AddressModel();
    [Key]
    public int addressId { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }

    public AddressCoordinatesModel() {}
    public AddressCoordinatesModel(AddressCoordinatesModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressCoordinatesModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }

}
