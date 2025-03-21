using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class WorkoutGoal
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Type { get; set; } // "Count", "Duration"

        [Required]
        public string Period { get; set; } // "Week", "Month", "Custom"

        [Range(1, int.MaxValue)]
        public int TargetValue { get; set; } // Для тренировок — количество или минуты

        public int Progress { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ApplicationUser User { get; set; }
    }
}
