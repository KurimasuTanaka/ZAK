using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Enums;



public enum StretchingStatus
{
    [Display(Name = "Протянута", Description = null)]
    Streched,
    [Display(Name = "Нетянута", Description = null)]
    NotSctreched,
    [Display(Name = "Будет протянута", Description = null)]
    WillBeStreched,
    [Display(Name = "Не выдавать", Description = null)]
    DoNotStrech

}

public enum BlackoutZone
{
    White,
    Gray,
    Black,
    Unknown
}

public enum EquipmentAccess
{
    [Display(Name = "ЖЕК", Description = null)]
    HousingOffice,
    [Display(Name = "Председатель, ограниченный", Description = null)]
    Chairman_limitted,
    [Display(Name = "Председатель, свободный", Description = null)]
    Chairman_free,
    [Display(Name = "Свободный", Description = null)]
    Free,
    [Display(Name = "Смешанный", Description = null)]
    Mixed,
    [Display(Name = "Не указан", Description = null)]
    Unknown

}
