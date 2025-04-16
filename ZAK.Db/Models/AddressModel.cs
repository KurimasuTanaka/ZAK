using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using BlazorApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Db.Models;


public class AddressModel 
{
    [Key]
    public int Id {get; set;}
    public string streetName  {get; set;} = String.Empty; 
    public string building {get; set;} = String.Empty;

    //Application
    //[ForeignKey("applicationId")]
    public List<ApplicationModel> applications {get; set;} = new List<ApplicationModel>();
    

    //District
    public DistrictModel? district {get; set;} = null;

    //Address alias
    public AddressAliasModel? addressAlias { get; set; } = null;

    //Priority
    public AddressPriorityModel? addressPriority { get; set; } = null;

    //Coordinates
    public AddressCoordinatesModel? coordinates { get; set; } = null;

    public EquipmentAccess equipmentAccess { get; set; } = EquipmentAccess.Unknown;

    public int blackoutGroup { get; set; } = 0;

    public AddressModel() {
        
    }
    public AddressModel(AddressModel addressModel)
    {
        foreach (PropertyInfo property in typeof(AddressModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(addressModel, null), null);
        }    
    }
}