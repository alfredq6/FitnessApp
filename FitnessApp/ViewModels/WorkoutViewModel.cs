using System.ComponentModel.DataAnnotations;

namespace FitnessApp.ViewModels
{
    public class WorkoutViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Укажите дату")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Укажите продолжительность")]
        [Range(1, int.MaxValue, ErrorMessage = "Продолжительность должна быть больше 0")]
        public int Duration { get; set; }
    }
}
