using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.DB;

public class BrigadeModel
{
    [Key]
    public int id { get; set; }

    public int brigadeNumber {get; set;}    

    public List<int> applicationsIds { get; set; } = new List<int>(new int[9]);

    public BrigadeModel() { }
    public BrigadeModel(BrigadeModel applicationModel)
    {
        foreach (PropertyInfo property in typeof(BrigadeModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(applicationModel, null), null);
        }
    }

}
