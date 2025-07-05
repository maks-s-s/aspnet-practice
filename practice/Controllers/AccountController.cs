using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using practice.Models;
using System.Linq;
using System.Threading.Tasks;

namespace practice.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet("/")]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost("/")]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
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
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (!ModelState.IsValid)
                return View();

            var user = new User { UserName = email, Email = email, EmailConfirmed = true };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
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
                return RedirectToAction("Index", "Home");

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                await _signInManager.SignInAsync(adminUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Content("Помилка при створенні адміністратора: " + string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        [Authorize]
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpGet("/accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
