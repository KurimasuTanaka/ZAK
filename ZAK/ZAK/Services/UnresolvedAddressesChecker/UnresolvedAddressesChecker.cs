using System;
using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.MapRoutesManager;

namespace ZAK.Services.UnresolvedAddressesChecker;

public class UnresolvedAddressesInfo
{
    public int unresolvedAddressesNumber { get; set; }
    public bool unresolvedAddressesExist { get; set; }
}

public class UnresolvedAddressesChecker : IUnresolvedAddressesChecker
{
    IAddressRepository _addressRepository;
    IMapRoutesManager _mapRoutesManager;

    public UnresolvedAddressesChecker(IAddressRepository addressRepository, IMapRoutesManager mapRoutesManager)
    {
        _addressRepository = addressRepository;
        _mapRoutesManager = mapRoutesManager;
    }

    public async Task<int> GetNumberOfUnresolvedAddresses()
    {
        List<Address> addresses = (await _addressRepository.GetAllAsync()).ToList();

        foreach (Address address in addresses)
        {
            address.resolved = _mapRoutesManager.CheckResolving(address, 150);
        }
        List<Address> unres = addresses.Select(a => a).Where(a => a.resolved is false).ToList();

        return unres.Count;
    }

    public int ResolveAddresses(List<Address> addresses)
    {
        foreach (Address address in addresses)
        {
            address.resolved = _mapRoutesManager.CheckResolving(address, 150);
        }
        List<Address> unres = addresses.Select(a => a).Where(a => a.resolved is false).ToList();

        return unres.Count;
    }

    public async Task<UnresolvedAddressesInfo> GetUnresolvedAddressesInfo()
    {
        return new UnresolvedAddressesInfo
        {
            unresolvedAddressesExist = await UnresolvedAddressesExist(),
            unresolvedAddressesNumber = await GetNumberOfUnresolvedAddresses()
        };
    }

    public async Task<bool> UnresolvedAddressesExist()
    {
        List<Address> addresses = (await _addressRepository.GetAllAsync()).ToList();

        foreach (Address address in addresses)
        {
            address.resolved = _mapRoutesManager.CheckResolving(address, 150);
        }
        List<Address> unres = addresses.Select(a => a).Where(a => a.resolved is false).ToList();

        return unres.Count > 0;
    }
}
