using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTracker.Web.Data;
using ReleaseTracker.Web.Models;

namespace ReleaseTracker.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ReleaseTrackerContext _context;

    public HomeController(ILogger<HomeController> logger, ReleaseTrackerContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalApps = await _context.Apps.CountAsync(a => a.IsActive);
        var totalReleases = await _context.Releases.CountAsync();

        var now = DateTime.Now;
        var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
        var releasesThisWeek = await _context.Releases
            .CountAsync(r => r.ReleaseDate >= startOfWeek);

        var recentReleases = await _context.Releases
            .Include(r => r.App)
            .Where(r => r.App != null && r.App.IsActive)
            .OrderByDescending(r => r.ReleaseDate)
            .Take(25)
            .ToListAsync();

        ViewBag.TotalApps = totalApps;
        ViewBag.TotalReleases = totalReleases;
        ViewBag.ReleasesThisWeek = releasesThisWeek;
        ViewBag.RecentReleases = recentReleases;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
