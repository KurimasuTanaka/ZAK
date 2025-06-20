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
    [Display(Name = "Звонить", Description = null)]
    Call_Required,
    [Display(Name = "Свободный", Description = null)]
    Free,
    [Display(Name = "Смотреть топологию", Description = null)]
    Check_Topology,
    [Display(Name = "Не указан", Description = null)]
    Unknown,
    [Display(Name = "Вадим", Description = null)]
    Vadim

}
