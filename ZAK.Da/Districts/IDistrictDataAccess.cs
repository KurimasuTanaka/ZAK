namespace BlazorApp.DA;

public interface IDistrictDataAccess
{
    public District GetDistrict(string districtName);
    public List<District> GetDistricts();
    public string GetDistrictColor(string districtName);
}
