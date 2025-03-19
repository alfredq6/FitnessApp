using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return View(workouts.Select(workout => new WorkoutViewModel
            {
                Date = workout.Date,
                Name = workout.Name,
                Duration = workout.Duration
            }).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new WorkoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Выводим ошибки валидации в консоль для диагностики
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Ошибка валидации: {error.ErrorMessage}");
                }
                return View(model); // Возвращаем форму с ошибками
            }

            var userId = _userManager.GetUserId(User);
            var workout = new Workout
            {
                UserId = userId,
                Date = model.Date,
                Name = model.Name,
                Duration = model.Duration
            };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Новое действие: Редактирование тренировки (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout == null)
            {
                return NotFound();
            }

            var model = new WorkoutViewModel
            {
                Date = workout.Date,
                Name = workout.Name,
                Duration = workout.Duration
            };
            return View(model);
        }

        // Новое действие: Редактирование тренировки (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout == null)
            {
                return NotFound();
            }

            workout.Date = model.Date;
            workout.Name = model.Name;
            workout.Duration = model.Duration;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine($"Получен id: {id}"); // Диагностика

            var userId = _userManager.GetUserId(User);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout == null)
            {
                Console.WriteLine("Тренировка не найдена или не принадлежит пользователю");
                return RedirectToAction("Index"); // Или можно вернуть ошибку
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Stats(string period = "all")
        {
            var userId = _userManager.GetUserId(User);
            var workoutsQuery = _context.Workouts.Where(w => w.UserId == userId);

            // Фильтрация по периоду
            DateTime now = DateTime.Now;
            switch (period.ToLower())
            {
                case "week":
                    workoutsQuery = workoutsQuery.Where(w => w.Date >= now.AddDays(-7));
                    break;
                case "month":
                    workoutsQuery = workoutsQuery.Where(w => w.Date >= now.AddMonths(-1));
                    break;
                case "all":
                default:
                    break;
            }

            var workouts = await workoutsQuery.ToListAsync();

            var culture = new CultureInfo("ru-RU");
            var stats = new WorkoutStatsViewModel
            {
                TotalWorkouts = workouts.Count,
                TotalDuration = workouts.Sum(w => w.Duration),
                AverageDuration = workouts.Any() ? workouts.Average(w => w.Duration) : 0,
                MostFrequentWorkout = workouts.Any()
                    ? workouts.GroupBy(w => w.Name).OrderByDescending(g => g.Count()).First().Key
                    : "Нет данных",
                FilterPeriod = period switch
                {
                    "week" => "за неделю",
                    "month" => "за месяц",
                    _ => "за всё время"
                },
                WorkoutsByDate = workouts.GroupBy(w => w.Date.Date)
                                        .ToDictionary(g => g.Key, g => g.Sum(w => w.Duration)),
                DurationByDayOfWeek = workouts.GroupBy(w => w.Date.DayOfWeek)
                                              .ToDictionary(
                                                  g => culture.DateTimeFormat.GetDayName(g.Key),
                                                  g => g.Sum(w => w.Duration)
                                              )
            };

            // Заполняем все дни недели
            var allDays = culture.DateTimeFormat.DayNames.Select(d => char.ToUpper(d[0]) + d.Substring(1)).ToArray();
            foreach (var day in allDays)
            {
                if (!stats.DurationByDayOfWeek.ContainsKey(day))
                {
                    stats.DurationByDayOfWeek[day] = 0;
                }
            }

            // Добавляем цели с прогрессом
            var goals = await _context.WorkoutGoals
                .Where(g => g.UserId == userId && !g.IsCompleted)
                .ToListAsync();
            foreach (var goal in goals)
            {
                var goalWorkouts = _context.Workouts
                    .Where(w => w.UserId == userId && w.Date >= goal.StartDate);
                if (goal.Period == "Week")
                    goalWorkouts = goalWorkouts.Where(w => w.Date <= goal.StartDate.AddDays(7));
                else if (goal.Period == "Month")
                    goalWorkouts = goalWorkouts.Where(w => w.Date <= goal.StartDate.AddMonths(1));

                var workoutsList = await goalWorkouts.ToListAsync();
                int progress = goal.Type == "Count" ? workoutsList.Count : workoutsList.Sum(w => w.Duration);

                stats.Goals.Add(new GoalProgress
                {
                    Id = goal.Id,
                    Type = goal.Type,
                    TargetValue = goal.TargetValue,
                    Period = goal.Period,
                    StartDate = goal.StartDate,
                    Progress = progress
                });
            }

            return View(stats);
        }
    }
}
