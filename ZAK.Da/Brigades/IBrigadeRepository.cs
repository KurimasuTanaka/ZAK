using System;
using ZAK.DA;

namespace ZAK.DA;

public interface IBrigadeRepository : IRepository<Brigade, int>
{
    Task<IEnumerable<Brigade>> GetAllWithScheduledApplicationInfoAsync();
}
