using System;

namespace ZAK.DA;

public interface IAddressRepository : IRepository<Address, int>
{
    Task CreateRangeAsync(IEnumerable<Address> addresses);
}
