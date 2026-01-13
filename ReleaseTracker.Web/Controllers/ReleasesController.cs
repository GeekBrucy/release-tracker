using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReleaseTracker.Web.Data;
using ReleaseTracker.Web.Models;
using ReleaseTracker.Web.Services;

namespace ReleaseTracker.Web.Controllers
{
    public class ReleasesController : Controller
    {
        private readonly ReleaseTrackerContext _context;
        private readonly ReleaseOptions _releaseOptions;
        private readonly IUserService _userService;

        public ReleasesController(ReleaseTrackerContext context, IOptions<ReleaseOptions> releaseOptions, IUserService userService)
        {
            _context = context;
            _releaseOptions = releaseOptions.Value;
            _userService = userService;
        }

        // GET: Releases
        public async Task<IActionResult> Index(int? appId, string environment, string status)
        {
            ViewData["CurrentAppId"] = appId;
            ViewData["CurrentEnvironment"] = environment;
            ViewData["CurrentStatus"] = status;

            // Populate filter dropdowns
            ViewBag.Apps = new SelectList(await _context.Apps.Where(a => a.IsActive).OrderBy(a => a.Name).ToListAsync(), "Id", "Name", appId);
            ViewBag.Environments = new SelectList(_releaseOptions.Environments, environment);
            ViewBag.Statuses = new SelectList(_releaseOptions.Statuses, status);

            var releases = _context.Releases
                .Include(r => r.App)
                .Where(r => r.App != null && r.App.IsActive)
                .AsQueryable();

            if (appId.HasValue)
            {
                releases = releases.Where(r => r.AppId == appId);
            }

            if (!string.IsNullOrEmpty(environment))
            {
                releases = releases.Where(r => r.Environment == environment);
            }

            if (!string.IsNullOrEmpty(status))
            {
                releases = releases.Where(r => r.Status == status);
            }

            return View(await releases.OrderByDescending(r => r.ReleaseDate).ToListAsync());
        }

        // GET: Releases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var release = await _context.Releases
                .Include(r => r.App)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (release == null)
            {
                return NotFound();
            }

            return View(release);
        }

        // GET: Releases/Create
        public async Task<IActionResult> Create(int? appId)
        {
            await PopulateDropdowns(appId);

            var release = new Release
            {
                ReleaseDate = DateTime.Now
            };

            if (appId.HasValue)
            {
                release.AppId = appId.Value;
            }

            return View(release);
        }

        // POST: Releases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppId,Version,ReleaseDate,ReleasedBy,Description,ReleaseNotes,Environment,Status")] Release release)
        {
            if (ModelState.IsValid)
            {
                release.CreatedDate = DateTime.Now;
                release.CreatedBy = _userService.GetCurrentUserName();

                _context.Add(release);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Release '{release.Version}' created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(release.AppId);
            return View(release);
        }

        // GET: Releases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var release = await _context.Releases.FindAsync(id);
            if (release == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(release.AppId);
            return View(release);
        }

        // POST: Releases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppId,Version,ReleaseDate,ReleasedBy,Description,ReleaseNotes,Environment,Status,CreatedDate,CreatedBy")] Release release)
        {
            if (id != release.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    release.ModifiedDate = DateTime.Now;
                    release.ModifiedBy = _userService.GetCurrentUserName();

                    _context.Update(release);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Release '{release.Version}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReleaseExists(release.Id))
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

            await PopulateDropdowns(release.AppId);
            return View(release);
        }

        // GET: Releases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var release = await _context.Releases
                .Include(r => r.App)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (release == null)
            {
                return NotFound();
            }

            return View(release);
        }

        // POST: Releases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var release = await _context.Releases.FindAsync(id);
            if (release != null)
            {
                _context.Releases.Remove(release);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Release has been deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReleaseExists(int id)
        {
            return _context.Releases.Any(e => e.Id == id);
        }

        private async Task PopulateDropdowns(int? selectedAppId = null)
        {
            ViewBag.Apps = new SelectList(
                await _context.Apps.Where(a => a.IsActive).OrderBy(a => a.Name).ToListAsync(),
                "Id",
                "Name",
                selectedAppId);

            ViewBag.Environments = new SelectList(_releaseOptions.Environments);
            ViewBag.Statuses = new SelectList(_releaseOptions.Statuses);
        }
    }
}
