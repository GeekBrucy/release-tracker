using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTracker.Web.Data;
using ReleaseTracker.Web.Models;
using ReleaseTracker.Web.Services;

namespace ReleaseTracker.Web.Controllers
{
    public class AppsController : Controller
    {
        private readonly ReleaseTrackerContext _context;
        private readonly IUserService _userService;

        public AppsController(ReleaseTrackerContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Apps
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var apps = _context.Apps
                .Where(a => a.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                apps = apps.Where(a => a.Name.Contains(searchString) ||
                                      (a.Description != null && a.Description.Contains(searchString)));
            }

            return View(await apps.OrderBy(a => a.Name).ToListAsync());
        }

        // GET: Apps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = await _context.Apps
                .Include(a => a.Releases.OrderByDescending(r => r.ReleaseDate).Take(10))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (app == null)
            {
                return NotFound();
            }

            return View(app);
        }

        // GET: Apps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Apps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] App app)
        {
            if (ModelState.IsValid)
            {
                app.CreatedDate = DateTime.Now;
                app.CreatedBy = _userService.GetCurrentUserName();
                app.IsActive = true;

                _context.Add(app);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Application '{app.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(app);
        }

        // GET: Apps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = await _context.Apps.FindAsync(id);
            if (app == null || !app.IsActive)
            {
                return NotFound();
            }
            return View(app);
        }

        // POST: Apps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedDate,CreatedBy,IsActive")] App app)
        {
            if (id != app.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(app);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Application '{app.Name}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppExists(app.Id))
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
            return View(app);
        }

        // GET: Apps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = await _context.Apps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (app == null || !app.IsActive)
            {
                return NotFound();
            }

            return View(app);
        }

        // POST: Apps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var app = await _context.Apps.FindAsync(id);
            if (app != null)
            {
                // Soft delete - just mark as inactive
                app.IsActive = false;
                _context.Update(app);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Application '{app.Name}' has been deactivated.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppExists(int id)
        {
            return _context.Apps.Any(e => e.Id == id);
        }
    }
}
