using System;
using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class AddressPriority : AddressPriorityModel
{

    public AddressPriority() { }
    public AddressPriority(AddressPriorityModel model) : base(model) { }

    public void Copy(AddressPriorityModel source)
    {
        foreach (PropertyInfo property in GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(source));
        }
    }
}
