using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public enum GenderType
    {
        NotSpecified = 0,
        Male = 1,
        Female = 2
    }

    public enum ActivityLevel
    {
        Sedentary = 1,
        LightlyActive = 2,
        ModeratelyActive = 3,
        VeryActive = 4,
        ExtremelyActive = 5
    }

    public enum WeightGoal
    {
        Maintain = 0,
        Lose = 1,
        Gain = 2
    }

    public class ApplicationUser : IdentityUser
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов")]
        public string? Name { get; set; }

        public GenderType Gender { get; set; }

        [Range(13, 120, ErrorMessage = "Возраст должен быть от 13 до 120 лет")]
        public int? Age { get; set; }

        [Range(30, 300, ErrorMessage = "Вес должен быть от 30 до 300 кг")]
        public double? Weight { get; set; }

        [Range(100, 250, ErrorMessage = "Рост должен быть от 100 до 250 см")]
        public int? Height { get; set; }

        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.ModeratelyActive;

        public WeightGoal WeightGoal { get; set; } = WeightGoal.Maintain;

        [Range(30, 300, ErrorMessage = "Желаемый вес должен быть от 30 до 300 кг")]
        public double? TargetWeight { get; set; }

        public DateTime? TargetDate { get; set; }
    }
}
