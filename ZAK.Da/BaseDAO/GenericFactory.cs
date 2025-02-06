using System;
using System.Linq.Expressions;

namespace ZAK.Da.BaseDAO;

public static class GenericFactory
{
    private static readonly Dictionary<Type, Delegate> _cache = new();

    public static T CreateInstance<T>(params object[] args)
    {
        var type = typeof(T);
        if (!_cache.TryGetValue(type, out var del))
        {
            var constructor = type.GetConstructor(new[] { typeof(int) });
            if (constructor == null)
                throw new InvalidOperationException("No suitable constructor found.");

            var param = Expression.Parameter(typeof(object[]), "args");
            var arg0 = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(0)), typeof(int));
            var newExp = Expression.New(constructor, arg0);
            var lambda = Expression.Lambda<Func<object[], T>>(newExp, param).Compile();

            _cache[type] = lambda;
            del = lambda;
        }

        return ((Func<object[], T>)del)(args);
    }
}