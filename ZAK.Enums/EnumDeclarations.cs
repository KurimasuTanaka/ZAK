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
