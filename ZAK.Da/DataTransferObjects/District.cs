using System.Diagnostics.CodeAnalysis;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class District : ZAK.Db.Models.DistrictModel
{
    public District() : base()
    {
    }

    public District(ZAK.Db.Models.DistrictModel data) : base(data)
    {
    }
}

public class DistrictsComparer : IEqualityComparer<District?>
{
    public bool Equals(District? x, District? y)
    {
        if (x is null || y is null) return false;
        if (x.name == y.name) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] District obj)
    {
        return obj.name.GetHashCode();
    }
}
