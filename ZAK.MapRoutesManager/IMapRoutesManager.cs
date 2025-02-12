using System;
using System.Numerics;
using BlazorApp.DA;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;

namespace ZAK.MapRoutesManager;

public interface IMapRoutesManager
{
    public Task<List<List<Vector2>>> GetRoutesAsync(IDaoBase<Brigade, BrigadeModel> brigadesDataAccess, IDaoBase<Application,ApplicationModel> applicationsDataAccess);
    public bool CheckResolving(float lat, float lon, float radius = 50);
    public bool CheckResolving(Address address, float radius = 50);
    public bool CheckConnection(Address address, float radius = 50);
}
