using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.Models;

namespace RMS.Web.Controllers;

public class BranchController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BranchController> _logger;

    public BranchController(ApplicationDbContext context, ILogger<BranchController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Branch
    public async Task<IActionResult> Index()
    {
        var branches = await _context.Branches
            .Include(b => b.Governorate)
            .Include(b => b.Area)
            .AsNoTracking()
            .ToListAsync();

        return View(branches);
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
}
