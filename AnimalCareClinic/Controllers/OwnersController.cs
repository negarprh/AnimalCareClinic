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
    public class OwnersController : Controller

    {
       
        private readonly AnimalCareClinicContext _context;

        public OwnersController(AnimalCareClinicContext context)
        {
            _context = context;
        }

        /// PRE: User has role Admin or Secretary.
        /// POST: Returns a list of all owners from the database.
        // GET: Owners
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owners.ToListAsync());
        }

        /// PRE: 'id' is a valid OwnerId.
        /// POST: Returns details of the selected owner or NotFound if it does not exist.
        // GET: Owners/Details/5
        [Authorize(Roles = "Admin,Secretary,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        /// PRE: User has role Admin or Secretary.
        /// POST: Returns an empty Create Owner form.
        // GET: Owners/Create
        [Authorize(Roles = "Admin,Secretary")]
        public IActionResult Create()
        {
            return View();
        }

        /// PRE: Owner model is bound from submitted form; ModelState may or may not be valid.
        /// POST: If ModelState is valid, inserts a new Owner record and redirects to Index;
        ///       otherwise redisplays the Create form with validation errors.
        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Create([Bind("OwnerId,FirstName,LastName,Address,PhoneNumber,Email")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }


        /// PRE: 'id' is a valid OwnerId.
        /// POST: Returns an Edit form pre-filled with owner data or NotFound if missing.
        // GET: Owners/Edit/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        /// PRE: 'id' matches owner.OwnerId; ModelState may or may not be valid.
        /// POST: If valid, updates the Owner record; otherwise shows Edit view with errors.
        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,FirstName,LastName,Address,PhoneNumber,Email")] Owner owner)
        {
            if (id != owner.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.OwnerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        /// PRE: 'id' is a valid OwnerId.
        /// POST: Returns a confirmation page showing owner info.
        // GET: Owners/Delete/5
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }


        /// PRE: 'id' is an existing OwnerId.
        /// POST: Removes that owner, saves changes, and redirects to Index.
        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Secretary")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner != null)
            {
                _context.Owners.Remove(owner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.OwnerId == id);
        }
    }
}
