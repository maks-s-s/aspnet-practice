using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using practice.Models;
using System.Linq;
using System.Threading.Tasks;

namespace practice.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("/")]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to Home/Index.");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost("/")]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login POST called with invalid model state.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", email);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user {Email}.", email);
                }
            }
            else
            {
                _logger.LogWarning("Login attempt with non-existent email {Email}.", email);
            }

            ModelState.AddModelError("", "Невірна електронна пошта або пароль");
            return View();
        }

        [AllowAnonymous]
        [HttpGet("/register")]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User already authenticated, redirecting to Home/Index.");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Register POST called with invalid model state.");
                return View();
            }

            var user = new User { UserName = email, Email = email, EmailConfirmed = true };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} registered successfully.", email);
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogWarning("Registration error for user {Email}: {Error}", email, error.Description);
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet("/createadmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin1!";

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists.");
                return RedirectToAction("Index", "Home");
            }

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                await _signInManager.SignInAsync(adminUser, false);
                _logger.LogInformation("Admin user created and logged in.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogError("Error creating admin user: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return Content("Помилка при створенні адміністратора: " + string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        [Authorize]
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User {UserName} logged out.", userName);
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpGet("/accessdenied")]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied page requested.");
            return View();
        }
    }
}
