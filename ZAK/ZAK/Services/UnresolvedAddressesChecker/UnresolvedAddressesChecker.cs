using System;
using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.MapRoutesManager;
using Microsoft.Extensions.Caching.Memory;

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
    IMemoryCache _memoryCache;

    public UnresolvedAddressesChecker(IAddressRepository addressRepository, IMapRoutesManager mapRoutesManager, IMemoryCache memoryCache)
    {
        _addressRepository = addressRepository;
        _mapRoutesManager = mapRoutesManager;
        _memoryCache = memoryCache;
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
        if( _memoryCache.TryGetValue("UnresolvedAddressesInfo", out UnresolvedAddressesInfo? cachedInfo))
        {
            if(cachedInfo is not null) return cachedInfo;
        }

        var newInfo = new UnresolvedAddressesInfo
        {
            unresolvedAddressesExist = await UnresolvedAddressesExist(),
            unresolvedAddressesNumber = await GetNumberOfUnresolvedAddresses()
        };

        _memoryCache.Set("UnresolvedAddressesInfo", newInfo, TimeSpan.FromMinutes(5));

        return newInfo;
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
