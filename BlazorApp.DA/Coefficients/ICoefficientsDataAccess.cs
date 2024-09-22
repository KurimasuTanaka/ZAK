using System;
using BlazorApp.DA;

namespace BlazorApp.DA;

public interface ICoefficientsDataAccess
{
    public Task<double> GetCoefficinetByName(string coefficient);
    public Task<bool> IsCoefficientInfinite(string coefficient);
    public Task<List<Coefficient>> GetCoefficients();
    public Task<Dictionary<string, double>> GetCoefficientsDictionary();
    public Task UpdateCoefficient(int id, double coefficient);
    public Task MakeParameterInfinite(int id);
}
