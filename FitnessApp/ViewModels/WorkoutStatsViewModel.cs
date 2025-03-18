namespace FitnessApp.ViewModels
{
    public class WorkoutStatsViewModel
    {
        public int TotalWorkouts { get; set; }
        public int TotalDuration { get; set; }
        public double AverageDuration { get; set; }
        public string MostFrequentWorkout { get; set; }
        public string FilterPeriod { get; set; }
        public Dictionary<DateTime, int> WorkoutsByDate { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<string, int> DurationByDayOfWeek { get; set; } = new Dictionary<string, int>
    {
        { "Понедельник", 0 }, { "Вторник", 0 }, { "Среда", 0 }, { "Четверг", 0 }, { "Пятница", 0 }, { "Суббота", 0 }, { "Воскресенье", 0 }
    };
        // Добавляем список целей с прогрессом
        public List<GoalProgress> Goals { get; set; } = new List<GoalProgress>();
    }

    public class GoalProgress
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int TargetValue { get; set; }
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public int Progress { get; set; } // Текущий прогресс (тренировки или минуты)
    }
}
