using System.Diagnostics;
using System.Threading.Tasks;
using AnimalCareClinic.Models;
using AnimalCareClinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalCareClinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AnimalCareClinicContext _context;

        public HomeController(ILogger<HomeController> logger, AnimalCareClinicContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// PRE: Application is running; user may or may not be authenticated.
        /// POST: Returns the home dashboard with welcome text and navigation links.
        public async Task<IActionResult> Index()
        {
            var model = new HomeDashboardViewModel();

            if (User.Identity?.IsAuthenticated == true)
            {
                model.TotalAnimals = await _context.Animals.CountAsync();
                model.TotalOwners = await _context.Owners.CountAsync();
                model.TotalAppointments = await _context.Appointments.CountAsync();
                model.AvailableSlots = await _context.Schedules.CountAsync(s => s.Status == "Available");

                var today = DateOnly.FromDateTime(DateTime.Today);
                model.TodaysAppointments = await _context.Appointments
                    .Include(a => a.Animal)
                    .ThenInclude(a => a.Owner)
                    .Include(a => a.Schedule)
                    .ThenInclude(s => s.Veterinarian)
                    .Where(a => a.AppointmentDate == today)
                    .OrderBy(a => a.AppointmentTime)
                    .Take(6)
                    .ToListAsync();
            }

            return View(model);
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
