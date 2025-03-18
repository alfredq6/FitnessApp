namespace FitnessApp.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Связь с пользователем через Identity
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; } // В минутах

        public ApplicationUser User { get; set; } // Навигационное свойство
    }
}
