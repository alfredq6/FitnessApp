using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.Services;
using FitnessApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly NutritionCalculator _nutritionCalculator;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, NutritionCalculator nutritionCalculator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _nutritionCalculator = nutritionCalculator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            var nutritionResult = _nutritionCalculator.Calculate(user);

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Gender = user.Gender,
                Age = user.Age,
                Weight = user.Weight,
                Height = user.Height,
                ActivityLevel = user.ActivityLevel,
                WeightGoal = user.WeightGoal,
                TargetWeight = user.TargetWeight,
                TargetDate = user.TargetDate,
                Bmr = nutritionResult.Bmr,
                MaintenanceCalories = nutritionResult.MaintenanceCalories,
                DailyCalorieAdjustment = nutritionResult.DailyCalorieAdjustment,
                Calories = nutritionResult.Calories,
                Protein = nutritionResult.Protein,
                Fat = nutritionResult.Fat,
                Carbs = nutritionResult.Carbs
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            if (model.WeightGoal != WeightGoal.Maintain)
            {
                if (!model.TargetWeight.HasValue)
                {
                    ModelState.AddModelError("TargetWeight", "Желаемый вес обязателен для цели сброса или набора веса.");
                }
                if (!model.TargetDate.HasValue)
                {
                    ModelState.AddModelError("TargetDate", "Дата достижения цели обязательна для цели сброса или набора веса.");
                }
                else if (model.TargetDate.Value <= DateTime.Now)
                {
                    ModelState.AddModelError("TargetDate", "Дата достижения цели должна быть в будущем.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.Name = model.Name;
            user.Gender = model.Gender;
            user.Age = model.Age;
            user.Weight = model.Weight;
            user.Height = model.Height;
            user.ActivityLevel = model.ActivityLevel;
            user.WeightGoal = model.WeightGoal;
            user.TargetWeight = model.WeightGoal == WeightGoal.Maintain ? null : model.TargetWeight;
            user.TargetDate = model.WeightGoal == WeightGoal.Maintain ? null : model.TargetDate;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.RemoveClaimsAsync(user, await _userManager.GetClaimsAsync(user));
                await _userManager.AddClaimAsync(user, new Claim("Name", user.Name ?? user.UserName));
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Профиль успешно обновлён!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
