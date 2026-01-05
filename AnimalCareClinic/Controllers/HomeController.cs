using AnimalCareClinic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace AnimalCareClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// PRE: Application is running; user may or may not be authenticated.
        /// POST: Returns the home dashboard with welcome text and navigation links.
        public IActionResult Index()
        {
            return View();
        }

        /// PRE: User requests privacy policy.
        /// POST: Returns the Privacy view.
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
