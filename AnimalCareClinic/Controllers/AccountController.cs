using AnimalCareClinic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AnimalCareClinic.Controllers
{

    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AnimalCareClinicContext _context;

        public AccountController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: None. User is not authenticated or wants to re-login.
        /// POST: Returns the Login view so the user can enter credentials.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// PRE: 'username' and 'password' are posted from the Login form.
        /// POST: If credentials exist in UserAccounts, creates an auth cookie with role claims
        ///       and redirects to Home/Index; otherwise returns Login view with error message.
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Simple check in DB
            var user = _context.UserAccounts
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Create claims (Name + Role)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to home or dashboard
            return RedirectToAction("Index", "Home");
        }

        /// PRE: User is currently authenticated.
        /// POST: Authentication cookie is cleared, and user is redirected to Account/Login.
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }


        /// PRE: User attempted to access a resource without the proper role.
        /// POST: Returns the AccessDenied view explaining the restriction.
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
