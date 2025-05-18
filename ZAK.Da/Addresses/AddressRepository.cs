using System;

namespace ZAK.DA;

public class AddressRepository : IAddressRepository
{
    
    AddressRepository()
    {
        // Constructor logic here
    }
    public Task CreateAsync(Address entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Address>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Address> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Address>> CreateRangeAsync(IEnumerable<Address> addresses)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Address entity)
    {
        throw new NotImplementedException();
    }
}
