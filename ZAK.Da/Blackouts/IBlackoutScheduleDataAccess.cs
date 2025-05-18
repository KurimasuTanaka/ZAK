using System;
using BlazorApp.Enums;
using ZAK.Db.Models;

namespace ZAK.DA;

public interface IBlackoutScheduleDataAccess
{
    public Task<BlackoutZone> GetZone(int group, int day, int time);
    public Task SetZone(int group, int day, int time, BlackoutZone zone);

    public Task<List<BlackoutModel>> GetBlackouts();
}
