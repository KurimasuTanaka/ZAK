using System;

namespace ZAK.DA;

public interface IAddressAliasDataAccess
{
    Task<List<AddressAlias>> GetAddressAliases();
    Task AddAddressAlias(AddressAlias addressAlias);
    Task UpdateAddressSteetAlias(int id, string streetAlias);
    Task UpdateAddressBuildingAlias(int id, string buildingAlias);
    Task DeleteAddressAlias(int id);
}
