namespace BlazorApp.DA;

using System.Collections.Generic;
using ZAK.Db;
using ZAK.Db.Models;

public class DistrictDataAccess : IDistrictDataAccess
{
    private  readonly BlazorAppDbContext _dbContext;

    public DistrictDataAccess(BlazorAppDbContext blazorAppDbContext) => _dbContext = blazorAppDbContext;

    public District GetDistrict(string districtName)
    {
        var district = _dbContext.districts.FirstOrDefault(district => district.name == districtName);

        if(district is null) return new District();
        else return new District(district);
    }

    public string GetDistrictColor(string districtName)
    {
        var district = _dbContext.districts.FirstOrDefault(district => district.name == districtName);

        if(district is null) return "White";
        else return district.color;
    }

    public List<District> GetDistricts()
    {
        return _dbContext.districts.Select(district => new District(district)).ToList();
    }
}
