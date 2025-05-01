using System;
using System.Linq.Expressions;

namespace ZAK.DAO;

public static class GenericFactory
{
    private static readonly Dictionary<Type, Delegate> _cache = new();

    public static TransObjT CreateInstance<TransObjT, EntityT>(params object[] args)
    {
        var type = typeof(TransObjT);
        if (!_cache.TryGetValue(type, out var del))
        {
            var constructor = type.GetConstructor([typeof(EntityT)]);
            if (constructor == null)
                throw new InvalidOperationException("No suitable constructor found.");

            var param = Expression.Parameter(typeof(object[]), "args");
            var arg0 = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(0)), typeof(EntityT));
            var newExp = Expression.New(constructor, arg0);
            var lambda = Expression.Lambda<Func<object[], TransObjT>>(newExp, param).Compile();

            _cache[type] = lambda;
            del = lambda;
        }

        return ((Func<object[], TransObjT>)del)(args);
    }
}