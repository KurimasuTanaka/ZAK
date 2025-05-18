using System;
using ZAK.DA;

namespace ZAK.Da.Brigades;

public interface IBrigadeRepository : IRepository<Brigade, int>
{
    Task<IEnumerable<Brigade>> GetAllWithScheduledApplicationInfoAsync();
}
