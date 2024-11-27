using System;

namespace BlazorApp.DA;

public interface IAddressesDataAccess
{
    public Task<double> GetPriorityByAddress(string streetName, string building);
    public Task UpdatePriority(int id, double priority);
    public Task<List<Address>> GetAddresses();
    public Task<List<Address>> GetAddressesWithoutLocation();
    public Task UpgdateAddresses(List<Address> addresses);
    public Task AddAddress(Address address);

    public Task SetBlackoutGroup(int id, int group);
}
