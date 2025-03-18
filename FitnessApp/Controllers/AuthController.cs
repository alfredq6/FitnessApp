using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Страница регистрации (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Обработка регистрации (POST)
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        // Страница входа (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Обработка входа (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
            return View();
        }

        // Выход
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}