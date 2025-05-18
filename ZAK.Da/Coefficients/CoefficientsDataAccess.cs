using Microsoft.EntityFrameworkCore;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class CoefficientsDataAccess : ICoefficientsDataAccess
{

    private  readonly ZakDbContext _dbContext;

    public CoefficientsDataAccess(ZakDbContext ZakDbContext) => _dbContext = ZakDbContext;

    public async Task<List<Coefficient>> GetCoefficients()
    {
        return await _dbContext.coefficients.Select(c => new Coefficient(c)).ToListAsync(); 
    }

    public async Task<Dictionary<string, double>> GetCoefficientsDictionary()
    {
        return await  _dbContext.coefficients.Select(c => new {c.parameter, c.coefficient}).
        ToDictionaryAsync(c => c.parameter, c => c.coefficient); 
    }


    public async Task<double> GetCoefficinetByName(string parameter)
    {
        CoefficientModel? coefficientModel = await _dbContext.coefficients.FirstOrDefaultAsync(c => c.parameter == parameter);
        if(coefficientModel is null) return 1;
        else return coefficientModel.coefficient;
    }

    public async Task<bool> IsCoefficientInfinite(string parameter)
    {
        CoefficientModel? coefficientModel = await _dbContext.coefficients.FirstOrDefaultAsync(c => c.parameter == parameter);
        if(coefficientModel is null) return false;
        else return coefficientModel.infinite;

    }

    public async Task MakeParameterInfinite(int id)
    {
        CoefficientModel? coefficientModel = await _dbContext.coefficients.FindAsync(id);
        if(coefficientModel is not null) 
        {
            coefficientModel.infinite = true;
            await   _dbContext.SaveChangesAsync(); 
        } 
    }

    public async Task UpdateCoefficient(int id, double coefficient)
    {
        CoefficientModel? coefficientModel = await _dbContext.coefficients.FindAsync(id);
        if(coefficientModel is not null) 
        {
            coefficientModel.coefficient = coefficient;
            await   _dbContext.SaveChangesAsync(); 
        } 
    }
}
