using System;

namespace ZAK.DA;

public interface IApplicationReporisory : IRepository<Application, int>
{
    Task<IEnumerable<Application>> GetAllWithIgnoringAsync();
    Task<IEnumerable<Application>> GetAllUpdatedAsync();
    Task CreateRangeAsync(IEnumerable<Application> entities);
    Task UpdateRangeAsync(IEnumerable<Application> entities);
    Task DeleteRangeAsync(IEnumerable<Application> entities);
}
