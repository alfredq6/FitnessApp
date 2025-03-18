using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class GoalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Список целей
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var goals = await _context.WorkoutGoals
                .Where(g => g.UserId == userId && !g.IsCompleted)
                .ToListAsync();

            // Подсчёт прогресса
            foreach (var goal in goals)
            {
                var workouts = _context.Workouts
                    .Where(w => w.UserId == userId && w.Date >= goal.StartDate);
                if (goal.Period == "Week")
                    workouts = workouts.Where(w => w.Date <= goal.StartDate.AddDays(7));
                else if (goal.Period == "Month")
                    workouts = workouts.Where(w => w.Date <= goal.StartDate.AddMonths(1));

                var workoutsList = await workouts.ToListAsync();
                int progress = goal.Type == "Count" ? workoutsList.Count : workoutsList.Sum(w => w.Duration);
                ViewData[$"Progress_{goal.Id}"] = progress;
            }

            return View(goals);
        }

        // Создание цели (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Создание цели (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string type, int targetValue, string period)
        {
            if (targetValue <= 0 || (type != "Count" && type != "Duration") || (period != "Week" && period != "Month"))
            {
                ModelState.AddModelError("", "Некорректные данные цели.");
                return View();
            }

            var userId = _userManager.GetUserId(User);
            var goal = new WorkoutGoal
            {
                UserId = userId,
                Type = type,
                TargetValue = targetValue,
                Period = period,
                StartDate = DateTime.Now,
                IsCompleted = false
            };

            _context.WorkoutGoals.Add(goal);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Пометить цель как выполненную
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _context.WorkoutGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal != null)
            {
                goal.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _context.WorkoutGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal == null)
            {
                return NotFound();
            }

            var model = new WorkoutGoalViewModel
            {
                Id = goal.Id,
                Type = goal.Type,
                TargetValue = goal.TargetValue,
                Period = goal.Period
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutGoalViewModel model)
        {
            if (model.TargetValue <= 0 || (model.Type != "Count" && model.Type != "Duration") || (model.Period != "Week" && model.Period != "Month"))
            {
                ModelState.AddModelError("", "Некорректные данные цели.");
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var goal = await _context.WorkoutGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (goal == null)
            {
                return NotFound();
            }

            goal.Type = model.Type;
            goal.TargetValue = model.TargetValue;
            goal.Period = model.Period;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
