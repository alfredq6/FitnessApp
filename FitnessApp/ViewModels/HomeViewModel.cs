namespace FitnessApp.ViewModels
{
    public class HomeViewModel
    {
        public int WeeklyWorkouts { get; set; }
        public int WeeklyDuration { get; set; }
        public List<GoalViewModel> Goals { get; set; } = new List<GoalViewModel>();
        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Fat { get; set; }
        public double? Carbs { get; set; }
        public double? WeightProgressPercentage { get; set; }
    }
}
