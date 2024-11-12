using System;
using BlazorApp.DB;

namespace BlazorApp.DA;

public class Coordinates : AddressCoordinatesModel
{
    public Coordinates() { }
    public Coordinates(AddressCoordinatesModel model) : base(model) { }
}
