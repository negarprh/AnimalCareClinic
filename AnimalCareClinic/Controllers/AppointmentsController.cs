using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AnimalCareClinic.Models;
using AnimalCareClinic.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AnimalCareClinic.Controllers
{
    // Global protection: only authenticated users
    [Authorize]
    public class AppointmentsController : Controller
    {
        private static readonly TimeOnly OpeningStart = new(9, 0);
        private static readonly TimeOnly OpeningEnd = new(17, 0);
        private static readonly HashSet<DayOfWeek> OpenDays =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        ];

        private readonly AnimalCareClinicContext _context;

        public AppointmentsController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: User is Admin, Secretary, or Veterinarian.
        /// POST: Returns list of all appointments with related animal and schedule.
        // GET: Appointments
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Index()
        {
            var animalClinicContext = _context.Appointments
                .Include(a => a.Animal)
                .ThenInclude(a => a.Owner)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Veterinarian);
            return View(await animalClinicContext.ToListAsync());
        }

        /// PRE: 'id' is a valid AppointmentId.
        /// POST: Returns appointment details or NotFound.
        // GET: Appointments/Details/5
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Animal)
                .ThenInclude(a => a.Owner)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        /// PRE: User is Admin or Secretary.
        /// POST: Returns Create form with dropdowns.
        // GET: Appointments/Create
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Create(int? veterinarianId, DateTime? appointmentDate)
        {
            var viewModel = new AppointmentFormViewModel
            {
                VeterinarianId = veterinarianId ?? 0,
                AppointmentDate = appointmentDate
            };

            await PopulateAppointmentFormLists(viewModel, null);
            return View(viewModel);
        }

        /// PRE: Appointment model is bound from form.
        /// POST: Inserts appointment and redirects to Index (logs validation errors but does not block).
        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Create(AppointmentFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                await PopulateAppointmentFormLists(new AppointmentFormViewModel(), null);
                return View(new AppointmentFormViewModel());
            }

            await PopulateAppointmentFormLists(viewModel, null);

            if (!ModelState.IsValid)
            {
                LogModelStateErrors("[Appointment Create]");
                return View(viewModel);
            }

            var schedule = await _context.Schedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(s => s.ScheduleId == viewModel.ScheduleId);

            if (schedule == null)
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleId), "Selected schedule slot was not found.");
                return View(viewModel);
            }

            if (!string.Equals(schedule.Status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleId), "Only available slots can be booked.");
                return View(viewModel);
            }

            if (schedule.VeterinarianId != viewModel.VeterinarianId)
            {
                ModelState.AddModelError(nameof(viewModel.VeterinarianId), "Selected schedule slot does not match the veterinarian.");
                return View(viewModel);
            }

            if (!TryValidateClinicHours(schedule.Date, schedule.TimeSlot, out var timeOnly, out var hoursError))
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleId), hoursError);
                return View(viewModel);
            }

            var appointment = new Appointment
            {
                ScheduleId = schedule.ScheduleId,
                AnimalId = viewModel.AnimalId,
                AppointmentDate = schedule.Date,
                AppointmentTime = timeOnly,
                Reason = viewModel.Reason,
                Status = "Booked"
            };

            schedule.Status = "Booked";
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Appointment booked successfully.";
            TempData["FlashType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is valid AppointmentId.
        /// POST: Returns Edit form pre-filled.
        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int? id, int? veterinarianId, DateTime? appointmentDate)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Veterinarian)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var viewModel = new AppointmentFormViewModel
            {
                AppointmentId = appointment.AppointmentId,
                VeterinarianId = veterinarianId ?? appointment.Schedule.VeterinarianId,
                AppointmentDate = appointmentDate ?? appointment.Schedule.Date.ToDateTime(TimeOnly.MinValue),
                ScheduleId = appointment.ScheduleId,
                AnimalId = appointment.AnimalId,
                Reason = appointment.Reason ?? string.Empty
            };

            await PopulateAppointmentFormLists(viewModel, appointment.ScheduleId);
            return View(viewModel);
        }

        /// PRE: 'id' matches updated.AppointmentId.
        /// POST: Updates appointment and redirects to Index.
        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int id, AppointmentFormViewModel viewModel)
        {
            if (viewModel == null || id != viewModel.AppointmentId)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            await PopulateAppointmentFormLists(viewModel, appointment.ScheduleId);

            if (!ModelState.IsValid)
            {
                LogModelStateErrors("[Appointment Edit]");
                return View(viewModel);
            }

            if (string.Equals(appointment.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(appointment.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Cancelled or completed appointments cannot be edited.");
                return View(viewModel);
            }

            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.ScheduleId == viewModel.ScheduleId);

            if (schedule == null)
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleId), "Selected schedule slot was not found.");
                return View(viewModel);
            }

            if (schedule.VeterinarianId != viewModel.VeterinarianId)
            {
                ModelState.AddModelError(nameof(viewModel.VeterinarianId), "Selected schedule slot does not match the veterinarian.");
                return View(viewModel);
            }

            if (!TryValidateClinicHours(schedule.Date, schedule.TimeSlot, out var timeOnly, out var hoursError))
            {
                ModelState.AddModelError(nameof(viewModel.ScheduleId), hoursError);
                return View(viewModel);
            }

            if (appointment.ScheduleId != viewModel.ScheduleId)
            {
                if (!string.Equals(schedule.Status, "Available", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(viewModel.ScheduleId), "Only available slots can be booked.");
                    return View(viewModel);
                }

                var oldSchedule = await _context.Schedules.FindAsync(appointment.ScheduleId);
                if (oldSchedule != null)
                {
                    oldSchedule.Status = "Available";
                }

                schedule.Status = "Booked";
                appointment.ScheduleId = schedule.ScheduleId;
            }
            else
            {
                schedule.Status = "Booked";
            }

            appointment.AnimalId = viewModel.AnimalId;
            appointment.Reason = viewModel.Reason;
            appointment.AppointmentDate = schedule.Date;
            appointment.AppointmentTime = timeOnly;
            appointment.Status = "Booked";

            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Appointment updated successfully.";
            TempData["FlashType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is valid AppointmentId.
        /// POST: Shows confirmation page.
        // GET: Appointments/Delete/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Animal)
                .ThenInclude(a => a.Owner)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        /// PRE: 'id' exists.
        /// POST: Removes appointment then redirects to Index.
        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment != null)
            {
                if (appointment.Schedule != null)
                {
                    appointment.Schedule.Status = "Available";
                }

                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Appointment deleted successfully.";
            TempData["FlashType"] = "warning";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Cancelled";
            if (appointment.Schedule != null)
            {
                appointment.Schedule.Status = "Available";
            }

            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Appointment cancelled. The slot is now available.";
            TempData["FlashType"] = "warning";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Complete(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Completed";
            if (appointment.Schedule != null)
            {
                appointment.Schedule.Status = "Completed";
            }

            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Appointment marked as completed.";
            TempData["FlashType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        private async Task PopulateAppointmentFormLists(AppointmentFormViewModel viewModel, int? currentScheduleId)
        {
            var veterinarians = await _context.Veterinarians
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            var animals = await _context.Animals
                .Include(a => a.Owner)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewData["VeterinarianId"] = new SelectList(veterinarians, "VeterinarianId", "DisplayName", viewModel.VeterinarianId);
            ViewData["AnimalId"] = new SelectList(animals, "AnimalId", "DisplayName", viewModel.AnimalId);

            var schedules = new List<Schedule>();
            if (viewModel.VeterinarianId > 0 && viewModel.AppointmentDate.HasValue)
            {
                var selectedDate = DateOnly.FromDateTime(viewModel.AppointmentDate.Value);
                var query = _context.Schedules
                    .Include(s => s.Veterinarian)
                    .Where(s => s.VeterinarianId == viewModel.VeterinarianId && s.Date == selectedDate);

                if (currentScheduleId.HasValue)
                {
                    query = query.Where(s => s.Status == "Available" || s.ScheduleId == currentScheduleId.Value);
                }
                else
                {
                    query = query.Where(s => s.Status == "Available");
                }

                schedules = await query
                    .OrderBy(s => s.Date)
                    .ThenBy(s => s.TimeSlot)
                    .ToListAsync();
            }

            ViewData["ScheduleId"] = new SelectList(schedules, "ScheduleId", "DisplayName", viewModel.ScheduleId);
            ViewData["ScheduleCount"] = schedules.Count;
            ViewData["SelectedVeterinarianName"] = veterinarians.FirstOrDefault(v => v.VeterinarianId == viewModel.VeterinarianId)?.DisplayName;
        }

        private static bool TryValidateClinicHours(DateOnly date, string timeSlot, out TimeOnly timeOnly, out string error)
        {
            timeOnly = default;
            error = string.Empty;

            if (!OpenDays.Contains(date.DayOfWeek))
            {
                error = "Clinic hours are Monday to Friday.";
                return false;
            }

            if (!TimeOnly.TryParseExact(timeSlot, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeOnly))
            {
                error = "Schedule time is invalid.";
                return false;
            }

            if (timeOnly < OpeningStart || timeOnly >= OpeningEnd)
            {
                error = "Selected slot is outside clinic hours (09:00-17:00).";
                return false;
            }

            return true;
        }

        private void LogModelStateErrors(string prefix)
        {
            foreach (var kvp in ModelState)
            {
                foreach (var err in kvp.Value.Errors)
                {
                    Console.WriteLine($"{prefix} {kvp.Key}: {err.ErrorMessage}");
                }
            }
        }
    }
}
