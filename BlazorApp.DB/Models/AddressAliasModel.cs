using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorApp.DB;

public class AddressAliasModel
{
    [Key]
    public int addressId { get; set; }

    public string streetAlias { get; set; } = String.Empty;
    public string buildingAlias { get; set; } = String.Empty;

    public AddressAliasModel() {}
    public AddressAliasModel(AddressAliasModel addressAliasModel)
    {
        foreach (PropertyInfo property in typeof(AddressAliasModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressAliasModel, null), null);
        }    
    }

}
