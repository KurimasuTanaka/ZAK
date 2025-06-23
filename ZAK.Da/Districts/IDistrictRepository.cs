using System;
using ZAK.DA;

namespace ZAK.DA;

public interface IDistrictRepository
{
    public Task<List<District>> GetAllAsync();
}
