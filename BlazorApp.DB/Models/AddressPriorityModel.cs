using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorApp.DB;

public class AddressPriorityModel
{
    [Key]
    public AddressModel address { get; set; } = new AddressModel();
    public int addressId { get; set; }


    public double priority { get; set; }

    public AddressPriorityModel() {}
    public AddressPriorityModel(AddressPriorityModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }
}
