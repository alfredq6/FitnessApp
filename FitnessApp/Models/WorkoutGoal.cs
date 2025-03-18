namespace FitnessApp.Models
{
    public class WorkoutGoal
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; } // "Count" (количество тренировок) или "Duration" (время)
        public int TargetValue { get; set; } // Целевое значение (тренировки или минуты)
        public string Period { get; set; } // "Week" (неделя), "Month" (месяц)
        public DateTime StartDate { get; set; } // Дата начала цели
        public bool IsCompleted { get; set; } // Флаг завершения

        public ApplicationUser User { get; set; }
    }
}
