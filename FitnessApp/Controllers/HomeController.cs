using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.Services;
using FitnessApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly NutritionCalculator _nutritionCalculator;

    public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, NutritionCalculator nutritionCalculator)
    {
        _userManager = userManager;
        _context = context;
        _nutritionCalculator = nutritionCalculator;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return View("Welcome");
        }

        var goals = await _context.WorkoutGoals
            .Where(g => g.UserId == user.Id)
            .ToListAsync();

        var lastWeekStart = DateTime.Now.AddDays(-7);
        var weeklyGoals = goals.Where(g => g.StartDate.HasValue && g.StartDate.Value >= lastWeekStart).ToList();

        var model = new HomeViewModel
        {
            WeeklyWorkouts = weeklyGoals
                .Where(g => g.Type == "Count")
                .Sum(g => g.Progress),
            WeeklyDuration = weeklyGoals
                .Where(g => g.Type == "Duration")
                .Sum(g => g.Progress),
            Goals = goals.Select(g => new GoalViewModel
            {
                Type = g.Type,
                Period = g.Period,
                TargetValue = g.TargetValue,
                Progress = g.Progress
            }).ToList()
        };

        var nutritionResult = _nutritionCalculator.Calculate(user);
        model.Calories = nutritionResult.Calories;
        model.Protein = nutritionResult.Protein;
        model.Fat = nutritionResult.Fat;
        model.Carbs = nutritionResult.Carbs;
        model.WeightProgressPercentage = nutritionResult.WeightProgressPercentage;

        return View(model);
    }
}
