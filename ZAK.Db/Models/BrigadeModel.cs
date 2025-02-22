using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace ZAK.Db.Models;

public class BrigadeModel
{
    [Key]
    public int id { get; set; }

    public int brigadeNumber {get; set;} 
    public int brigadeSlotsCount { get; set; }  = 9;

    public List<ScheduledApplicationModel> scheduledApplications { get; set; } = new List<ScheduledApplicationModel>();

    public BrigadeModel() { }
    public BrigadeModel(BrigadeModel applicationModel)
    {
        foreach (PropertyInfo property in typeof(BrigadeModel).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(applicationModel, null), null);
        }
    }

}
