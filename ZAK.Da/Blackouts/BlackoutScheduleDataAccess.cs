using System;
using BlazorApp.Enums;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class BlackoutScheduleDataAccess : IBlackoutScheduleDataAccess
{
    BlazorAppDbContext _dbContext;
    
    public BlackoutScheduleDataAccess(BlazorAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BlackoutModel>> GetBlackouts()
    {
        return await _dbContext.shutdowns.ToListAsync();
    }

    public async Task<BlackoutZone> GetZone(int group, int day, int time)
    {
        BlackoutModel? blackoutModel = await _dbContext.shutdowns.FirstOrDefaultAsync(s => s.group == group && s.day == day && s.time == time);
        if(blackoutModel is not null) return blackoutModel.zone;
        else return BlackoutZone.Unknown; 
    }

    public async Task SetZone(int group, int day, int time, BlackoutZone zone)
    {
        BlackoutModel? blackoutModel = await _dbContext.shutdowns.FirstOrDefaultAsync(s => s.group == group && s.day == day && s.time == time);
        if(blackoutModel is not null) 
        {
            blackoutModel.zone = zone;
            await _dbContext.SaveChangesAsync();
        } else 
        {
            blackoutModel = new BlackoutModel() { group = group, day = day, time = time, zone = zone };
            await _dbContext.shutdowns.AddAsync(blackoutModel);
            await _dbContext.SaveChangesAsync();
        }

    }
}
