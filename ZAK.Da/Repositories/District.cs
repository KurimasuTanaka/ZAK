using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class District : DistrictModel
{
    public District() : base()
    {
    }

    public District(DistrictModel data) : base(data)
    {
    }
}
