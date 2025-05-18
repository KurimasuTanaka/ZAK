using System;
using ZAK.DA;

namespace ZAK.Services.UnresolvedAddressesChecker;

public interface IUnresolvedAddressesChecker
{
    public Task<bool> UnresolvedAddressesExist();
    public Task<int> GetNumberOfUnresolvedAddresses();
    public int ResolveAddresses(List<Address> addresses);

    public Task<UnresolvedAddressesInfo> GetUnresolvedAddressesInfo();
}
