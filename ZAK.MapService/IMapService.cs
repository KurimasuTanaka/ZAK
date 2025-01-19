using System;
using System.Numerics;

namespace ZAK.MapService;

public interface IMapService
{
    public Task InitializeMap();
    public Task DrawRoute(List<Vector2> route);
}
