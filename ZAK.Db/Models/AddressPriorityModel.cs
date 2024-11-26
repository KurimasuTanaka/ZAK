using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ZAK.Db.Models;

public class AddressPriorityModel
{
    //[ForeignKey("addressId")]
    public AddressModel address { get; set; } = new AddressModel();
    [Key]
    public int addressId { get; set; }


    public double priority { get; set; }

    public AddressPriorityModel() {}
    public AddressPriorityModel(AddressPriorityModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressPriorityModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }
}
