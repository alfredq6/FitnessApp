namespace FitnessApp.ViewModels
{
    public class HomeViewModel
    {
        public int WeeklyWorkouts { get; set; }
        public int WeeklyDuration { get; set; }
        public List<GoalProgress> Goals { get; set; } = new List<GoalProgress>();
    }
}
