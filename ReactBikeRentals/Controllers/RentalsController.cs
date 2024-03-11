using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReactBikes.Data;
using ReactBikes.Models;

namespace ReactBikes.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ReactBikesContext _context;
        private readonly UserManager<ReactBikesUser> _userManager;
        public RentalsController(ReactBikesContext context, UserManager<ReactBikesUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var _UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var Rentals = User.IsInRole("Manager") ? 
                _context.Rental.Include(r => r.Bike).Include(r => r.User) :                                 // Manager
                _context.Rental.Include(r => r.Bike).Include(r => r.User).Where(r => r.UserID == _UserID);  // User

            Rentals = Rentals.OrderBy(b => b.Returned);
            
            return View(await Rentals.ToListAsync());
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.Bike)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Return/5
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.Bike)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);

            if (rental == null)
            {
                return NotFound();
            }
            rental.DateReturned = DateTime.Now;
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, int? RentalId, [Bind("RentalId, UserID, BikeID, DateFrom, DateTo, DateReturned, Returned, Rating, Comments")] Rental rental)
        {
            if (_context.Rental == null)
            {
                return NotFound();
            }

            if (rental == null)
            {
                return NotFound();
            }
            
            try
            {
                if (rental.Rating < 1) rental.Rating = 1;
                if (rental.Rating > 5) rental.Rating = 5;

                if (rental.Comments == null)
                    rental.Comments = "n/a";

                rental.Returned = true;
                rental.Bike = _context.Bike.Find(rental.BikeID);
                if (rental.Bike != null)
                {
                    rental.Bike.Available = true;
                    _context.Update(rental.Bike);
                }

                _context.Update(rental);
                await _context.SaveChangesAsync();

                // Update average rating
                if (rental.Bike != null)
                {
                    int average_rating = 0;
                    var all_rentals = _context.Rental.Where(m => m.BikeID == rental.Bike.BikeId);
                    foreach (var r in all_rentals)
                    {
                        average_rating += r.Rating;
                    }
                    average_rating = average_rating / all_rentals.Count();
                    rental.Bike.Rating = average_rating;
                    _context.Update(rental.Bike);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(rental.RentalId))
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

        // GET: Rentals/Create
        public IActionResult Create(int? BikeID)
        {
            Rental rental = new Rental();
            rental.DateFrom = DateTime.Now;
            rental.DateTo = DateTime.Now;
            rental.Comments = "";
            rental.Rating = 1;
            rental.BikeID = BikeID;
            rental.Bike = _context.Bike.Find(BikeID);

            return View(rental);
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,UserID,BikeID,DateFrom,DateTo,DateReturned,Returned,Rating,Comments")] Rental rental)
        {
            rental.Comments = "";
            rental.Rating = 1;
            rental.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            rental.Bike = _context.Bike.Find(rental.BikeID);
            if (rental.Bike != null)
            {
                rental.Bike.Available = false;
                _context.Update(rental.Bike);
            }
            _context.Add(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["BikeID"] = new SelectList(_context.Bike, "BikeId", "ModelName", rental.BikeID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Email", rental.UserID);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,UserID,BikeID,DateFrom,DateTo,DateReturned,Returned,Rating,Comments")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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
            ViewData["BikeID"] = new SelectList(_context.Bike, "BikeId", "ModelName", rental.BikeID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Email", rental.UserID);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.Bike)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rental == null)
            {
                return Problem("Entity set 'ReactBikeRentalsContext.Rental'  is null.");
            }
            var rental = await _context.Rental.FindAsync(id);
            if (rental != null)
            {
                _context.Rental.Remove(rental);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
          return _context.Rental.Any(e => e.RentalId == id);
        }
    }
}
