using System;

namespace BlazorApp.DA.Addresses;

public interface IAddressesDataAccess
{
    public Task<double> GetPriorityByAddress(string streetName, string building);
    public Task UpdatePriority(int id, double priority);
    public Task<List<Address>> GetAddresses();
    public Task AddAddress(Address address);
}
