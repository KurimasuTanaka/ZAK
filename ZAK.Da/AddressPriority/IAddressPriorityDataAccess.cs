using System;

namespace BlazorApp.DA;

public interface IAddressPriorityDataAccess
{
    Task<List<AddressPriority>> GetAddressPriorities();
    Task<double> GetAddressPriority(int id);
    Task UpdatePriority(int id, double priority);
}
