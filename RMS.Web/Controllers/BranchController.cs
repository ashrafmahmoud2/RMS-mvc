using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;

namespace RMS.Web.Controllers;



public class BranchController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BranchController> _logger;
    private readonly IMapper _mapper;

    public BranchController(ApplicationDbContext context, ILogger<BranchController> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        // Get all branches with their governorates
        var branches = await _context.Branches
            .AsNoTracking()
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .Include(b => b.BranchWorkingHours)
            .Include(b => b.WorkingHourExceptions)
            .OrderBy(b => b.Governorate!.NameAr)
            .ThenBy(b => b.NameAr)
            .ToListAsync();

        // Map to view models and calculate working hours status
        var branchViewModels = new List<BranchViewModel>();

        foreach (var branch in branches)
        {
            var branchViewModel = _mapper.Map<BranchViewModel>(branch);

            // Calculate working hours status directly in controller
            branchViewModel.IsCurrentlyOpen = IsBranchOpenNow(branch);
            branchViewModel.WorkingHoursStatus = GetWorkingHoursStatus(branch);
            branchViewModel.WorkingHoursText = GetWorkingHoursText(branch);

            branchViewModels.Add(branchViewModel);
        }

        // Group by governorate
        var groupedBranches = branchViewModels
            .GroupBy(b => new { b.GovernorateNameAr })
            .Select(g => new GovernorateWithBranchesViewModel
            {
                Name = g.Key.GovernorateNameAr,
                Branches = g.OrderBy(b => b.Name).ToList()
            })
            .OrderBy(g => g.Name)
            .ToList();

        var viewModel = new BranchIndexViewModel
        {
            Governorates = groupedBranches
        };

        return View(viewModel);
    }


    // GET: Branch/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var branch = await _context.Branches
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .Include(b => b.BranchWorkingHours)
            .Include(b => b.WorkingHourExceptions)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (branch == null)
            return NotFound();

        return View(branch);
    }

    // GET: Branch/Create
    public IActionResult Create()
    {
        ViewData["Governorates"] = _context.Governorates.ToList();
        ViewData["Areas"] = _context.Areas.ToList();
        return View(new Branch());
    }

    // POST: Branch/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Branch branch)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Governorates"] = _context.Governorates.ToList();
            ViewData["Areas"] = _context.Areas.ToList();
            return View(branch);
        }

        // Validation: Governorate & Area must exist
        var governorateExists = await _context.Governorates.AnyAsync(g => g.Id == branch.GovernorateId);
        var areaExists = await _context.Areas.AnyAsync(a => a.Id == branch.AreaId);

        if (!governorateExists || !areaExists)
        {
            ModelState.AddModelError("", "Invalid Governorate or Area selected.");
            return View(branch);
        }

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Branch/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch == null)
            return NotFound();

        ViewData["Governorates"] = _context.Governorates.ToList();
        ViewData["Areas"] = _context.Areas.ToList();
        return View(branch);
    }

    // POST: Branch/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Branch branch)
    {
        if (id != branch.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["Governorates"] = _context.Governorates.ToList();
            ViewData["Areas"] = _context.Areas.ToList();
            return View(branch);
        }

        try
        {
            _context.Update(branch);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Branches.Any(b => b.Id == branch.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Branch/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var branch = await _context.Branches
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (branch == null)
            return NotFound();

        return View(branch);
    }

    // POST: Branch/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch != null)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

        private bool IsBranchOpenNow(Branch branch)
        {
            if (branch.IsBusy)
                return false;

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            // Check for exceptions first
            var exception = branch.WorkingHourExceptions
                .FirstOrDefault(e => e.Date == today);

            if (exception != null)
            {
                return IsTimeInRange(currentTime.ToTimeSpan(), exception.OpeningTime, exception.ClosingTime);
            }

            // Check regular working hours
            var todayWorkingHours = branch.BranchWorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == now.DayOfWeek);

            if (todayWorkingHours == null)
                return false;

            return IsTimeInRange(currentTime.ToTimeSpan(), todayWorkingHours.OpeningTime, todayWorkingHours.ClosingTime);
        }

        private string GetWorkingHoursStatus(Branch branch)
        {
            var isOpen = IsBranchOpenNow(branch);
            return isOpen ? "مفتوح" : "مغلق";
        }

        private string GetWorkingHoursText(Branch branch)
        {
            if (branch.IsBusy)
                return "مغلق مؤقتاً";

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            // Check for exceptions first
            var exception = branch.WorkingHourExceptions
                .FirstOrDefault(e => e.Date == today);

            if (exception != null)
            {
                if (IsTimeInRange(currentTime.ToTimeSpan(), exception.OpeningTime, exception.ClosingTime))
                {
                    return $"مفتوح حتى {FormatTime(exception.ClosingTime)}";
                }
                else
                {
                    return $"مغلق - {exception.ExceptionNameAr}";
                }
            }

            // Check regular working hours
            var todayWorkingHours = branch.BranchWorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == now.DayOfWeek);

            if (todayWorkingHours == null)
            {
                // Find next day that has working hours
                var nextWorkingDay = GetNextWorkingDay(branch.BranchWorkingHours, now.DayOfWeek);
                if (nextWorkingDay != null)
                {
                    return $"مغلق - يفتح {GetDayName(nextWorkingDay.DayOfWeek)} {FormatTime(nextWorkingDay.OpeningTime)}";
                }
                return "مغلق";
            }

            if (IsTimeInRange(currentTime.ToTimeSpan(), todayWorkingHours.OpeningTime, todayWorkingHours.ClosingTime))
            {
                return $"مفتوح حتى {FormatTime(todayWorkingHours.ClosingTime)}";
            }
            else if (currentTime.ToTimeSpan() < todayWorkingHours.OpeningTime)
            {
                return $"مغلق - يفتح اليوم {FormatTime(todayWorkingHours.OpeningTime)}";
            }
            else
            {
                // Find tomorrow or next working day
                var nextWorkingDay = GetNextWorkingDay(branch.BranchWorkingHours, now.DayOfWeek);
                if (nextWorkingDay != null)
                {
                    var dayName = nextWorkingDay.DayOfWeek == now.AddDays(1).DayOfWeek ? "غداً" : GetDayName(nextWorkingDay.DayOfWeek);
                    return $"مغلق - يفتح {dayName} {FormatTime(nextWorkingDay.OpeningTime)}";
                }
                return "مغلق";
            }
        }

        private static bool IsTimeInRange(TimeSpan currentTime, TimeSpan openTime, TimeSpan closeTime)
        {
            // Handle cases where close time is next day (e.g., 22:00 - 02:00)
            if (closeTime < openTime)
            {
                return currentTime >= openTime || currentTime <= closeTime;
            }
            return currentTime >= openTime && currentTime <= closeTime;
        }

        private static BranchWorkingHour? GetNextWorkingDay(ICollection<BranchWorkingHour> workingHours, DayOfWeek currentDay)
        {
            for (int i = 1; i <= 7; i++)
            {
                var nextDay = (DayOfWeek)(((int)currentDay + i) % 7);
                var workingHour = workingHours.FirstOrDefault(wh => wh.DayOfWeek == nextDay);
                if (workingHour != null)
                    return workingHour;
            }
            return null;
        }

        private static string FormatTime(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var ampm = hours >= 12 ? "م" : "ص";

            if (hours > 12) hours -= 12;
            if (hours == 0) hours = 12;

            return minutes == 0 ? $"{hours} {ampm}" : $"{hours}:{minutes:D2} {ampm}";
        }

        private static string GetDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الإثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                DayOfWeek.Saturday => "السبت",
                _ => ""
            };
        }
    }
