using System;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class Coordinates : AddressCoordinatesModel
{
    public Coordinates() { }
    public Coordinates(AddressCoordinatesModel model) : base(model) { }
}
