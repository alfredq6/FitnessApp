using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public enum GenderType
    {
        NotSpecified = 0, // "Не указано" как дефолтное значение
        Male = 1,         // "Мужской"
        Female = 2        // "Женский"
    }

    public class ApplicationUser : IdentityUser
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов")]
        public string? Name { get; set; }

        public GenderType Gender { get; set; } // Enum, по умолчанию 0 (NotSpecified)

        [Range(13, 120, ErrorMessage = "Возраст должен быть от 13 до 120 лет")]
        public int? Age { get; set; }

        [Range(30, 300, ErrorMessage = "Вес должен быть от 30 до 300 кг")]
        public double? Weight { get; set; }

        [Range(100, 250, ErrorMessage = "Рост должен быть от 100 до 250 см")]
        public int? Height { get; set; }
    }
}
