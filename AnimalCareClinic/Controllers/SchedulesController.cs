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
    [Authorize(Roles = "Admin")]
    public class SchedulesController : Controller
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

        public SchedulesController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: User is Admin or Secretary.
        /// POST: Returns list of schedules with related veterinarian.
        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            var animalClinicContext = _context.Schedules.Include(s => s.Veterinarian);
            return View(await animalClinicContext.ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        /// PRE: User is Admin or Secretary.
        /// POST: Returns Create form with Veterinarian dropdown.
        // GET: Schedules/Create
        public IActionResult Create()
        {
            var viewModel = new ScheduleFormViewModel
            {
                Date = DateTime.Today,
                Status = "Available"
            };

            PopulateScheduleFormLists(viewModel.VeterinarianId, viewModel.TimeSlot, viewModel.Status);
            return View(viewModel);
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                PopulateScheduleFormLists(null, null, "Available");
                return View(new ScheduleFormViewModel { Status = "Available" });
            }

            var validation = await ValidateScheduleAsync(viewModel, null);
            if (!validation.IsValid)
            {
                foreach (var message in validation.Errors)
                {
                    ModelState.AddModelError(string.Empty, message);
                }
            }

            if (!ModelState.IsValid)
            {
                LogModelStateErrors("[Schedule Create]");
                PopulateScheduleFormLists(viewModel.VeterinarianId, viewModel.TimeSlot, viewModel.Status);
                return View(viewModel);
            }

            var schedule = new Schedule
            {
                VeterinarianId = viewModel.VeterinarianId,
                Date = validation.Date,
                TimeSlot = validation.TimeSlot,
                Status = viewModel.Status
            };

            _context.Add(schedule);
            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Schedule created successfully.";
            TempData["FlashType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var viewModel = new ScheduleFormViewModel
            {
                ScheduleId = schedule.ScheduleId,
                VeterinarianId = schedule.VeterinarianId,
                Date = schedule.Date.ToDateTime(TimeOnly.MinValue),
                TimeSlot = schedule.TimeSlot,
                Status = schedule.Status
            };

            PopulateScheduleFormLists(viewModel.VeterinarianId, viewModel.TimeSlot, viewModel.Status);
            return View(viewModel);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScheduleFormViewModel viewModel)
        {
            if (viewModel == null || id != viewModel.ScheduleId)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var validation = await ValidateScheduleAsync(viewModel, id);
            if (!validation.IsValid)
            {
                foreach (var message in validation.Errors)
                {
                    ModelState.AddModelError(string.Empty, message);
                }
            }

            if (!ModelState.IsValid)
            {
                LogModelStateErrors("[Schedule Edit]");
                PopulateScheduleFormLists(viewModel.VeterinarianId, viewModel.TimeSlot, viewModel.Status);
                return View(viewModel);
            }

            schedule.VeterinarianId = viewModel.VeterinarianId;
            schedule.Date = validation.Date;
            schedule.TimeSlot = validation.TimeSlot;
            schedule.Status = viewModel.Status;

            try
            {
                _context.Update(schedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(schedule.ScheduleId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["FlashMessage"] = "Schedule updated successfully.";
            TempData["FlashType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();

            TempData["FlashMessage"] = "Schedule deleted successfully.";
            TempData["FlashType"] = "warning";

            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }

        private void PopulateScheduleFormLists(int? veterinarianId, string? timeSlot, string? status)
        {
            ViewData["VeterinarianId"] = new SelectList(
                _context.Veterinarians.OrderBy(v => v.LastName).ThenBy(v => v.FirstName),
                "VeterinarianId",
                "DisplayName",
                veterinarianId);

            ViewData["TimeSlots"] = BuildTimeSlotOptions(timeSlot);

            var statusOptions = new List<string> { "Available", "Booked", "Completed", "Unavailable" };
            ViewData["StatusOptions"] = new SelectList(statusOptions, status);
        }

        private static List<SelectListItem> BuildTimeSlotOptions(string? selected)
        {
            var options = new List<SelectListItem>();
            for (var time = OpeningStart; time < OpeningEnd; time = time.AddMinutes(30))
            {
                var text = time.ToString("HH:mm", CultureInfo.InvariantCulture);
                options.Add(new SelectListItem { Value = text, Text = text, Selected = text == selected });
            }

            if (!string.IsNullOrWhiteSpace(selected) && options.All(o => o.Value != selected))
            {
                options.Insert(0, new SelectListItem { Value = selected, Text = selected, Selected = true });
            }

            return options;
        }

        private async Task<(bool IsValid, DateOnly Date, string TimeSlot, List<string> Errors)> ValidateScheduleAsync(
            ScheduleFormViewModel viewModel,
            int? scheduleId)
        {
            var errors = new List<string>();
            var normalizedTimeSlot = viewModel.TimeSlot?.Trim() ?? string.Empty;
            var parsedDate = viewModel.Date ?? DateTime.MinValue;

            if (!viewModel.Date.HasValue)
            {
                errors.Add("Date is required.");
                return (false, default, normalizedTimeSlot, errors);
            }

            if (!OpenDays.Contains(parsedDate.DayOfWeek))
            {
                errors.Add("Clinic hours are Monday to Friday.");
            }

            if (!TimeOnly.TryParseExact(
                    normalizedTimeSlot,
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedTime))
            {
                errors.Add("Time slot must be in HH:mm format.");
            }
            else if (parsedTime < OpeningStart || parsedTime >= OpeningEnd)
            {
                errors.Add("Time slot must be within clinic hours (09:00-17:00).");
            }
            else
            {
                normalizedTimeSlot = parsedTime.ToString("HH:mm", CultureInfo.InvariantCulture);
            }

            var dateOnly = DateOnly.FromDateTime(parsedDate);
            if (viewModel.VeterinarianId > 0 && !string.IsNullOrWhiteSpace(normalizedTimeSlot))
            {
                var exists = await _context.Schedules.AnyAsync(s =>
                    s.VeterinarianId == viewModel.VeterinarianId &&
                    s.Date == dateOnly &&
                    s.TimeSlot == normalizedTimeSlot &&
                    (!scheduleId.HasValue || s.ScheduleId != scheduleId.Value));

                if (exists)
                {
                    errors.Add("This veterinarian already has a slot for the selected date and time.");
                }
            }

            return (errors.Count == 0, dateOnly, normalizedTimeSlot, errors);
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
