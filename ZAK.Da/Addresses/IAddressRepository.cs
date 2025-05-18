using System;

namespace ZAK.DA;

public interface IAddressRepository : IRepository<Address, int>
{
    Task<IEnumerable<Address>> CreateRangeAsync(IEnumerable<Address> addresses);
}
