using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimalCareClinic.Models;
using Microsoft.AspNetCore.Authorization;

namespace AnimalCareClinic.Controllers
{
    // Global protection: only authenticated users
    [Authorize]
    public class AppointmentsController : Controller
    {
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
                .Include(a => a.Schedule);
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
                .Include(a => a.Schedule)
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
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId");
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId");
            return View();
        }

        /// PRE: Appointment model is bound from form.
        /// POST: Inserts appointment and redirects to Index (logs validation errors but does not block).
        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (appointment == null)
            {
                ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId");
                ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId");
                return View();
            }

            // log errors if binding/validation failed
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[Appointment Create] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is valid AppointmentId.
        /// POST: Returns Edit form pre-filled.
        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId", appointment.AnimalId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId", appointment.ScheduleId);
            return View(appointment);
        }

        /// PRE: 'id' matches updated.AppointmentId.
        /// POST: Updates appointment and redirects to Index.
        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int id, Appointment updated)
        {
            if (updated == null || id != updated.AppointmentId)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // log validation errors (for debug) but do not block saving
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[Appointment Edit] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            // force update fields
            appointment.ScheduleId = updated.ScheduleId;
            appointment.AnimalId = updated.AnimalId;
            appointment.AppointmentDate = updated.AppointmentDate;
            appointment.AppointmentTime = updated.AppointmentTime;
            appointment.Reason = updated.Reason;
            appointment.Status = updated.Status;

            await _context.SaveChangesAsync();
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
                .Include(a => a.Schedule)
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
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
