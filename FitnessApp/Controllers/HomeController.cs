using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [AllowAnonymous] // Доступно всем
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);
            var viewModel = new HomeViewModel();

            // Краткая статистика за неделю
            var weekStart = DateTime.Now.AddDays(-7);
            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId && w.Date >= weekStart)
                .ToListAsync();
            viewModel.WeeklyWorkouts = workouts.Count;
            viewModel.WeeklyDuration = workouts.Sum(w => w.Duration);

            // Прогресс целей
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
                viewModel.Goals.Add(new GoalProgress
                {
                    Id = goal.Id,
                    Type = goal.Type,
                    TargetValue = goal.TargetValue,
                    Period = goal.Period,
                    StartDate = goal.StartDate,
                    Progress = progress
                });
            }

            return View("Dashboard", viewModel);
        }

        // Для неавторизованных — приветственная страница
        return View("Welcome");
    }
}
