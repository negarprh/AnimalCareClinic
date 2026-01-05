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
    public class AnimalsController : Controller
    {
        private readonly AnimalCareClinicContext _context;

        public AnimalsController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: User is Admin, Secretary, or Veterinarian.
        /// POST: Returns list of all animals including their owners.
        // GET: Animals
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Index()
        {
            var animalClinicContext = _context.Animals.Include(a => a.Owner);
            return View(await animalClinicContext.ToListAsync());
        }

        /// PRE: 'id' is a valid AnimalId.
        /// POST: Returns animal details including owner, or NotFound if not found.
        // GET: Animals/Details/5
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        /// PRE: User is Admin or Secretary.
        /// POST: Returns the Create Animal form with Owner dropdown populated.
        // GET: Animals/Create
        [Authorize(Roles = "Admin,Secretary")]
        public IActionResult Create()
        {
            // dropdown with owners
            ViewData["OwnerId"] = new SelectList(
                _context.Owners.OrderBy(o => o.LastName).ThenBy(o => o.FirstName),
                "OwnerId",
                "DisplayName");
            return View();
        }

        /// PRE: Animal model is bound from form; ModelState may or may not be valid.
        /// POST: If data present, inserts new Animal and redirects to Index;
        ///       otherwise redisplays Create view.
        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Create(Animal animal)
        {
            // simple null guard
            if (animal == null)
            {
                ViewData["OwnerId"] = new SelectList(
                    _context.Owners.OrderBy(o => o.LastName).ThenBy(o => o.FirstName),
                    "OwnerId",
                    "DisplayName");
                return View();
            }

            // log validation errors (if any) but do not block saving
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[Animal Create] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is a valid AnimalId.
        /// POST: Returns Edit form with animal data; NotFound if missing.
        // GET: Animals/Edit/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            ViewData["OwnerId"] = new SelectList(
                _context.Owners.OrderBy(o => o.LastName).ThenBy(o => o.FirstName),
                "OwnerId",
                "DisplayName",
                animal.OwnerId);
            return View(animal);
        }

        /// PRE: 'id' matches updated.AnimalId; user is Admin/Secretary.
        /// POST: Updates existing animal and redirects to Index.
        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int id, Animal updated)
        {
            if (updated == null || id != updated.AnimalId)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            // log validation errors (for debugging only)
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        Console.WriteLine($"[Animal Edit] {kvp.Key}: {err.ErrorMessage}");
                    }
                }
            }

            // force update fields
            animal.OwnerId = updated.OwnerId;
            animal.Name = updated.Name;
            animal.Species = updated.Species;
            animal.Age = updated.Age;
            animal.Gender = updated.Gender;
            animal.MedicalHistory = updated.MedicalHistory;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// PRE: 'id' is a valid AnimalId.
        /// POST: Shows confirmation page with basic animal info.
        // GET: Animals/Delete/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        /// PRE: 'id' is an existing AnimalId.
        /// POST: Removes the record and redirects to Index.
        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.AnimalId == id);
        }
    }
}
