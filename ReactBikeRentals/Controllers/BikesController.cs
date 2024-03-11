using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReactBikes.Data;
using ReactBikes.Models;

namespace ReactBikes.Controllers
{
    public class BikesController : Controller
    {
        private readonly ReactBikesContext _context;

        public BikesController(ReactBikesContext context)
        {
            _context = context;
            ReactBikesDBInitialize.AddTestData(_context);
        }

        // GET: Bikes
        public async Task<IActionResult> Index(string? modelSearchString, string? colourSearchString, string? locationSearchString)
        {
            var bikes = from m in _context.Bike
                        select m;

            if (!String.IsNullOrEmpty(modelSearchString))
            {
                bikes = bikes.Where(b => b.ModelName!.Contains(modelSearchString));
            }
            if (!String.IsNullOrEmpty(colourSearchString))
            {
                bikes = bikes.Where(b => b.Colour!.Contains(colourSearchString));
            }
            if (!String.IsNullOrEmpty(locationSearchString))
            {
                bikes = bikes.Where(b => b.LocationPostcode!.Contains(locationSearchString));
            }
            bikes = bikes.OrderByDescending(b => b.Available);

            return bikes != null ? 
                          View(await bikes.ToListAsync()) :
                          Problem("Entity set 'ReactBikeRentalsContext.Bike'  is null.");
        }

        // GET: Bikes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike
                .FirstOrDefaultAsync(m => m.BikeId == id);
            if (bike == null)
            {
                return NotFound();
            }

            return View(bike);
        }

        // GET: Bikes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bikes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BikeId,ModelName,Colour,LocationPostcode,Rating,Available")] Bike bike)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bike);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bike);
        }

        // GET: Bikes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike.FindAsync(id);
            if (bike == null)
            {
                return NotFound();
            }
            return View(bike);
        }

        // POST: Bikes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,int selectedRating, [Bind("BikeId,ModelName,Colour,LocationPostcode,Rating,Available")] Bike bike)
        {
            if (id != bike.BikeId)
            {
                return NotFound();
            }

            var selectedRatingB = selectedRating;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BikeExists(bike.BikeId))
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
            return View(bike);
        }

        // GET: Bikes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike
                .FirstOrDefaultAsync(m => m.BikeId == id);
            if (bike == null)
            {
                return NotFound();
            }

            return View(bike);
        }

        // POST: Bikes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bike == null)
            {
                return Problem("Entity set 'ReactBikeRentalsContext.Bike'  is null.");
            }
            var bike = await _context.Bike.FindAsync(id);
            if (bike != null)
            {
                _context.Bike.Remove(bike);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool BikeExists(int id)
        {
          return (_context.Bike?.Any(e => e.BikeId == id)).GetValueOrDefault();
        }
    }
}
