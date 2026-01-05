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
    [Authorize]
    public class VisitHistoriesController : Controller
    {
        private readonly AnimalCareClinicContext _context;

        public VisitHistoriesController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: User is Admin, Secretary, or Veterinarian.
        /// POST: Returns list of visit histories with related animal, appointment, and vet.
        // GET: VisitHistories
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Index()
        {
            var animalClinicContext = _context.VisitHistories
                .Include(v => v.Animal)
                .Include(v => v.Appointment)
                .Include(v => v.Veterinarian);

            return View(await animalClinicContext.ToListAsync());
        }

        /// PRE: 'id' is a valid VisitId.
        /// POST: Returns visit details or NotFound.
        // GET: VisitHistories/Details/5
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitHistory = await _context.VisitHistories
                .Include(v => v.Animal)
                .Include(v => v.Appointment)
                .Include(v => v.Veterinarian)
                .FirstOrDefaultAsync(m => m.VisitId == id);

            if (visitHistory == null)
            {
                return NotFound();
            }

            return View(visitHistory);
        }

        /// PRE: User is Admin or Veterinarian.
        /// POST: Returns Create VisitHistory form with dropdowns for Animal, Appointment, Vet.
        // GET: VisitHistories/Create
        [Authorize(Roles = "Admin,Veterinarian")]
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId");
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarians, "VeterinarianId", "VeterinarianId");
            return View();
        }

        /// PRE: VisitHistory model posted; ModelState may or may not be valid.
        /// POST: Inserts visit record and redirects to Index (logs validation errors but does not block).
        // POST: VisitHistories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Create(VisitHistory visitHistory)
        {
            if (visitHistory == null)
            {
                ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId");
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
                ViewData["VeterinarianId"] = new SelectList(_context.Veterinarians, "VeterinarianId", "VeterinarianId");
                return View();
            }

            // log validation errors (for debugging only)
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[VisitHistory Create] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            _context.VisitHistories.Add(visitHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is a valid VisitId.
        /// POST: Returns Edit form with existing visit data; NotFound if missing.
        // GET: VisitHistories/Edit/5
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitHistory = await _context.VisitHistories.FindAsync(id);
            if (visitHistory == null)
            {
                return NotFound();
            }

            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId", visitHistory.AnimalId);
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", visitHistory.AppointmentId);
            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarians, "VeterinarianId", "VeterinarianId", visitHistory.VeterinarianId);

            return View(visitHistory);
        }

        /// PRE: 'id' matches updated.VisitId; user is Admin/Veterinarian.
        /// POST: Updates visit record and redirects to Index.
        // POST: VisitHistories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Edit(int id, VisitHistory updated)
        {
            if (updated == null || id != updated.VisitId)
            {
                return NotFound();
            }

            var visitHistory = await _context.VisitHistories.FindAsync(id);
            if (visitHistory == null)
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
                        Console.WriteLine($"[VisitHistory Edit] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            // force update fields
            visitHistory.AppointmentId = updated.AppointmentId;
            visitHistory.AnimalId = updated.AnimalId;
            visitHistory.VeterinarianId = updated.VeterinarianId;
            visitHistory.VisitDate = updated.VisitDate;
            visitHistory.Diagnosis = updated.Diagnosis;
            visitHistory.Treatment = updated.Treatment;
            visitHistory.Prescription = updated.Prescription;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is valid VisitId.
        /// POST: Shows confirmation page.
        // GET: VisitHistories/Delete/5
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitHistory = await _context.VisitHistories
                .Include(v => v.Animal)
                .Include(v => v.Appointment)
                .Include(v => v.Veterinarian)
                .FirstOrDefaultAsync(m => m.VisitId == id);

            if (visitHistory == null)
            {
                return NotFound();
            }

            return View(visitHistory);
        }

        /// PRE: 'id' exists.
        /// POST: Removes visit record and redirects to Index.
        // POST: VisitHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitHistory = await _context.VisitHistories.FindAsync(id);
            if (visitHistory != null)
            {
                _context.VisitHistories.Remove(visitHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitHistoryExists(int id)
        {
            return _context.VisitHistories.Any(e => e.VisitId == id);
        }
    }
}
