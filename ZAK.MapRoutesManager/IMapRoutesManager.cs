using System;
using System.Numerics;
using ZAK.DA;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.MapRoutesManager;

public interface IMapRoutesManager
{
    public Task<List<List<Vector2>>> GetRoutesAsync();
    public bool CheckResolving(float lat, float lon, float radius = 50);
    public bool CheckResolving(Address address, float radius = 50);
    public bool CheckConnection(Address address, float radius = 50);
}
