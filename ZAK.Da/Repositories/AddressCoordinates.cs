using System;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class AddressCoordinates : AddressCoordinatesModel
{
    public AddressCoordinates() { }
    public AddressCoordinates(AddressCoordinatesModel model) : base(model) { }
}
