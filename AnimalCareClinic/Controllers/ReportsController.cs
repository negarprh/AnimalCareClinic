using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimalCareClinic.Models;
using AnimalCareClinic.ViewModels;

namespace AnimalCareClinic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly AnimalCareClinicContext _context;

        public ReportsController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: Optional 'year' and 'month' parameters; defaults to current month if null.
        /// POST: Queries appointments and visit histories for that month,
        ///       computes totals (booked, cancelled, completed) and vets' workload,
        ///       and returns Monthly view with a MonthlyReportVM model.
        // GET: /Reports/Monthly?year=2025&month=12
        public async Task<IActionResult> Monthly(int? year, int? month)
        {
            var today = DateTime.Today;
            int y = year ?? today.Year;
            int m = month ?? today.Month;

            // Use DateOnly because your columns are DateOnly
            var start = new DateOnly(y, m, 1);
            var end = start.AddMonths(1);

            // All appointments in the selected month
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate >= start && a.AppointmentDate < end)
                .ToListAsync();

            var vm = new MonthlyReportViewModel
            {
                Year = y,
                Month = m,
                TotalAppointments = appointments.Count,
                BookedCount = appointments.Count(a => a.Status == "Booked"),
                CancelledCount = appointments.Count(a => a.Status == "Cancelled"),
                CompletedCount = appointments.Count(a => a.Status == "Completed")
            };

            // Vet workload: how many visits each veterinarian completed in that month
            vm.VetWorkloads = await _context.VisitHistories
                .Where(v => v.VisitDate >= start && v.VisitDate < end)
                .Include(v => v.Veterinarian)
                .GroupBy(v => v.Veterinarian)
                .Select(g => new VetWorkloadItem
                {
                    VeterinarianId = g.Key.VeterinarianId,
                    VeterinarianName = g.Key.FirstName + " " + g.Key.LastName,
                    VisitCount = g.Count()
                })
                .ToListAsync();

            return View(vm);
        }
    }
}
