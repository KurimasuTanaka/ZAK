using System;
using BlazorApp.Enums;

namespace BlazorApp.DA;

public interface IBlackoutScheduleDataAccess
{
    public Task<BlackoutZone> GetZone(int group, int day, int time);
    public Task SetZone(int group, int day, int time, BlackoutZone zone);
}
