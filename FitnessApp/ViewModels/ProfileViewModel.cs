using FitnessApp.Models;

namespace FitnessApp.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public GenderType Gender { get; set; }
        public int? Age { get; set; }
        public double? Weight { get; set; }
        public int? Height { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public WeightGoal WeightGoal { get; set; }
        public double? TargetWeight { get; set; }
        public DateTime? TargetDate { get; set; }

        public double? Bmr { get; set; }
        public double? MaintenanceCalories { get; set; }
        public double? DailyCalorieAdjustment { get; set; }
        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Fat { get; set; }
        public double? Carbs { get; set; }
    }
}
