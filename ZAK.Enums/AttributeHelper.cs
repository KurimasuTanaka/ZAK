namespace BlazorApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class AttributeHelper
{
    public static string GetDisplayName<T>(this T Enum) where T : Enum
    {
        var attr = Enum.GetEnumAttribute<T, DisplayAttribute>();
        return attr?.Name ?? string.Empty;
    }
    public static string GetDescription<T>(this T Enum) where T : Enum
    {
        var attr = Enum.GetEnumAttribute<T, DisplayAttribute>();
        return attr?.Description ?? string.Empty;
    }
    public static DisplayAttribute? GetDisplayAttribute<T>(this T Enum) where T : Enum
        => Enum.GetEnumAttribute<T, DisplayAttribute>();
    public static TAttribute? GetEnumAttribute<TEnum, TAttribute>(this TEnum Enum)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        var memberInfo = typeof(TEnum).GetMember(Enum.ToString());
        if (memberInfo.Length == 0) return null;
        return memberInfo[0].GetCustomAttribute<TAttribute>();
    }
}