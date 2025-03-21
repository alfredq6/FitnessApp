using FitnessApp.Models;

namespace FitnessApp.Services
{
    public class NutritionResult
    {
        public double Bmr { get; set; }
        public double MaintenanceCalories { get; set; }
        public double? DailyCalorieAdjustment { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public double? WeightProgressPercentage { get; set; }
    }

    public class NutritionCalculator
    {
        public NutritionResult Calculate(ApplicationUser user)
        {
            var result = new NutritionResult();

            if (!user.Weight.HasValue || !user.Height.HasValue || !user.Age.HasValue)
            {
                return result;
            }

            result.Bmr = user.Gender == GenderType.Male
                ? 10 * user.Weight.Value + 6.25 * user.Height.Value - 5 * user.Age.Value + 5
                : 10 * user.Weight.Value + 6.25 * user.Height.Value - 5 * user.Age.Value - 161;

            double activityMultiplier = user.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2,
                ActivityLevel.LightlyActive => 1.375,
                ActivityLevel.ModeratelyActive => 1.55,
                ActivityLevel.VeryActive => 1.725,
                ActivityLevel.ExtremelyActive => 1.9,
                _ => 1.55
            };

            result.MaintenanceCalories = result.Bmr * activityMultiplier;
            result.Calories = result.MaintenanceCalories;

            if (user.WeightGoal != WeightGoal.Maintain && user.TargetWeight.HasValue && user.TargetDate > DateTime.Now)
            {
                double targetWeight = user.TargetWeight.Value;
                double currentWeight = user.Weight.Value;
                double weightDifference = targetWeight - currentWeight;
                double daysToTarget = (user.TargetDate.Value - DateTime.Now).TotalDays;

                if (daysToTarget > 0)
                {
                    double caloriesPerKg = 7700;
                    double totalCalorieAdjustment = weightDifference * caloriesPerKg;
                    result.DailyCalorieAdjustment = totalCalorieAdjustment / daysToTarget;
                    result.Calories += result.DailyCalorieAdjustment.Value;

                    double initialDifference = targetWeight - currentWeight;
                    double currentProgress = initialDifference - weightDifference;
                    result.WeightProgressPercentage = Math.Abs(initialDifference) > 0
                        ? Math.Min(Math.Abs(currentProgress / initialDifference) * 100, 100)
                        : 0;
                }
            }

            result.Protein = (result.Calories * 0.3) / 4;
            result.Fat = (result.Calories * 0.3) / 9;
            result.Carbs = (result.Calories * 0.4) / 4;

            return result;
        }
    }
}
