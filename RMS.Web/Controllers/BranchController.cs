using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels;
using RMS.Web.Core.ViewModels.Branches;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;
using RMS.Web.Services.Interfaces;

namespace RMS.Web.Controllers;

/*


stop in
1. fix crate working houre 
2. in edit why its send the old value to server
3. make details view
4. make btn form chnage status open close
5. add clinet ui add alart of close time like drik it or jahaz






*/

public class BranchController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BranchController> _logger;
    private readonly IMapper _mapper;
    private readonly IBranchService _branchService;

    public BranchController(ApplicationDbContext context, ILogger<BranchController> logger, 
        IMapper mapper, IBranchService branchService
                       )
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
       _branchService = branchService;
    }




    public async Task<IActionResult> Index()
    {
        var branches = await _branchService.GetAllBranchesAsync();

        var groupedBranches = branches
            // 1. Group by a composite key including ID and both names
            .GroupBy(b => new
            {
                b.GovernorateId,
                b.GovernorateNameAr,
                b.GovernorateNameEn
            })
            .Select(g => new GovernorateWithBranchesViewModel
            {
                Id = g.Key.GovernorateId,
                NameAr = g.Key.GovernorateNameAr,
                NameEn = g.Key.GovernorateNameEn, // <-- Set NameEn correctly

                Branches = g.OrderBy(b => b.NameAr).ToList()
            })
            // Order by Arabic name (or English, depending on preference)
            .OrderBy(g => g.NameAr)
            .ToList();

        var viewModel = new BranchIndexViewModel
        {
            Governorates = groupedBranches
        };

        return View(viewModel);
    }

    public async Task<IActionResult> AdminIndex()
    {
        var viewModel = await _branchService.GetBranchesGroupedByGovernorateAsync();
        return View(viewModel);
    }


    public async Task<IActionResult> Details(int id)
    {
        var branch = await _branchService.GetBranchByIdAsync(id);
        if (branch == null)
            return NotFound();

        return View(branch);
    }


    //public static BranchFormViewModel CreateMockBranchFormViewModel()
    //{
    //    // For IFormFile, we typically mock the behavior in tests. 
    //    // For a data object, we initialize the collection as empty or with null/mock files.
    //    var mockFileCollection = new List<IFormFile>();

    //    return new BranchFormViewModel
    //    {


    //        // --- Basic Info ---
    //        NameEn = "Central City Hub",
    //        NameAr = "الفرع الرئيسي للمدينة",

    //        // --- Location IDs ---
    //        AreaId = 6,
    //        GovernorateId = 2,

    //        // --- Select List Data (essential for MVC View rendering) ---
    //        AreaList = new List<SelectListItem>
    //        {
    //            new SelectListItem { Value = "5", Text = "Downtown Area", Selected = true },
    //            new SelectListItem { Value = "6", Text = "North Suburb" },
    //        },
    //        GovernorateList = new List<SelectListItem>
    //        {
    //            new SelectListItem { Value = "1", Text = "First Gov" },
    //            new SelectListItem { Value = "2", Text = "Capital Gov", Selected = true },
    //        },

    //        // --- Address & Contact ---
    //        AddressEn = "123 Technology Street, Central City",
    //        AddressAr = "شارع التكنولوجيا 123، المدينة المركزية",
    //        Phone = "96512345678", // Valid international-style phone number

    //        // --- Financial & Delivery ---
    //        MaxCashOnDeliveryAllowed = 750.00m,
    //        DeliveryFee = 2.50m,
    //        DeliveryTimeInMinutes = 45,

    //        // --- Operational Status ---
    //        MaxAllowedOrdersInDay = 800,
    //        IsBusy = false,
    //        IsOpen = true,

    //        // --- Image Management ---
    //        NewImageFiles = mockFileCollection,
    //        ExistingBranchImagePaths = new List<string>
    //        {
    //            "https://cdn.example.com/images/branch_42_main.jpg",
    //            "https://cdn.example.com/images/branch_42_interior.jpg"
    //        },

    //        // --- Nested Collections ---
    //        WorkingHours = new List<BranchWorkingHoursFormViewModel>
    //        {
    //            // Monday (1) 9:00 to 17:00
    //            new BranchWorkingHoursFormViewModel { DayOfWeek = DayOfWeek.Sunday }, 
    //            // Tuesday (2) 9:00 to 17:00
    //            new BranchWorkingHoursFormViewModel { DayOfWeek = DayOfWeek.Monday, 
    //                OpeningTime = new TimeSpan(9, 0, 0), 
    //                ClosingTime = new TimeSpan(17, 0, 0)
    //            }
    //            // Add more as needed for testing list population
    //        },
    //        WorkingHourExceptions = new List<BranchExceptionHoursFormViewModel>
    //        {
    //            // Mock an exception for today
    //           // new BranchExceptionHoursFormViewModel { Date = DateTime.Today}
    //        }
    //    };
    //}

    public IActionResult Create()
    {
        var model = new BranchFormViewModel();
        PopulateDropdowns(model);
        return PartialView("Form", model);
    }

    public static BranchFormViewModel EditMockBranchFormViewModel()
    {
        // For IFormFile, we typically mock the behavior in tests. 
        // For a data object, we initialize the collection as empty or with null/mock files.
        var mockFileCollection = new List<IFormFile>();

        return new BranchFormViewModel
        {
            // Crucial for EDIT scenario
            Id = 1,

            // --- Basic Info ---
            NameEn = "Central City Hubjjjj",
            NameAr = "jjjjالفرع الرئيسي للمدينة",

            // --- Location IDs (Updated AreaId to 6, matching the new selected list item) ---
            AreaId = 6,
            GovernorateId = 2,

            // --- Select List Data (essential for MVC View rendering) ---
            AreaList = new List<SelectListItem>
            {
                // Unselected
                new SelectListItem { Value = "5", Text = "Downtown Area", Selected = false },
                // Selected value now matches AreaId = 6
                new SelectListItem { Value = "6", Text = "North Suburb", Selected = true },
            },
            GovernorateList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "First Gov" },
                new SelectListItem { Value = "2", Text = "Capital Gov", Selected = true },
            },

            // --- Address & Contact ---
            AddressEn = "123 Technology Street, Central Cityjjjj",
            AddressAr = "شارع التكنولوجيا 123، المدينة المركزيةjjj",
            Phone = "96512345678", // Valid international-style phone number

            // --- Financial & Delivery ---
            MaxCashOnDeliveryAllowed = 750.00m,
            DeliveryFee = 2.50m,
            DeliveryTimeInMinutes = 45,

            // --- Operational Status ---
            MaxAllowedOrdersInDay = 800,
            IsBusy = false,
            IsOpen = true,

            // --- Image Management ---
            NewImageFiles = mockFileCollection,
            ExistingBranchImagePaths = new List<string>
            {
                "https://cdn.example.com/images/branch_42_main.jpg",
                "https://cdn.example.com/images/branch_42_interior.jpg"
            },

            // --- Nested Collections ---
            WorkingHours = new List<BranchWorkingHoursFormViewModel>
            {
                // Sunday: Uses default times (9:00 - 17:00)
                new BranchWorkingHoursFormViewModel { DayOfWeek = DayOfWeek.Sunday }, 
                // Monday: Explicitly set times
                new BranchWorkingHoursFormViewModel {
                    DayOfWeek = DayOfWeek.Monday,
                    OpeningTime = new TimeSpan(10, 0, 0),
                    ClosingTime = new TimeSpan(22, 0, 0) // Open later
                }
            },
            WorkingHourExceptions = new List<BranchExceptionHoursFormViewModel>
            {
                // Mock an exception: Christmas Day, branch closed all day (times are null)
             
            }
        };
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BranchFormViewModel viewModel )
    {
        if (!ModelState.IsValid)
        {
            PopulateDropdowns(viewModel);
            return View("Form", viewModel);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var (success, message, branchId) = await _branchService.CreateBranchAsync(viewModel, userId);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Details), new { id = branchId });
        }

        ModelState.AddModelError(string.Empty, message);
        PopulateDropdowns(viewModel);
        return View("Form", viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = await _branchService.GetBranchForEditAsync(id);
        if (viewModel == null)
            return NotFound();

        PopulateDropdowns(viewModel);
        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BranchFormViewModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            PopulateDropdowns(viewModel);
            return View("Form", viewModel);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var (success, message) = await _branchService.UpdateBranchAsync(id, viewModel, userId);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Edit), new { id });
        }

        ModelState.AddModelError(string.Empty, message);
        PopulateDropdowns(viewModel);
        return View("Form", viewModel);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var branch = await _branchService.GetBranchByIdAsync(id);
        if (branch == null)
            return NotFound();

        return View(branch);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (success, message) = await _branchService.DeleteBranchAsync(id);

        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, isOpen) = await _branchService.ToggleBranchStatusAsync(id);

        if (!success)
            return NotFound();

        return Ok(new { success = true, isOpen });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBusy(int id)
    {
        var (success, isBusy) = await _branchService.ToggleBranchBusyAsync(id);

        if (!success)
            return NotFound();

        return Ok(new { success = true, isBusy });
    }

    [HttpGet]
    public async Task<IActionResult> GetAreas(int governorateId)
    {
        var areas = await _context.Areas
            .Where(a => a.GovernorateId == governorateId)
            .Select(a => new { id = a.Id, nameAr = a.NameAr, nameEn = a.NameEn })
            .ToListAsync();

        return Ok(areas);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteImage(int branchId, string imageUrl)
    {
        try
        {
            var branch = await _context.Branches
                .Include(b => b.BranchImages)
                .FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                return NotFound();

            var image = branch.BranchImages.FirstOrDefault(img => img.ImageUrl == imageUrl);
            if (image != null)
            {
                _context.BranchImages.Remove(image);
                await _context.SaveChangesAsync();

                // Delete from R2 (handled by service)
                // await _r2Service.DeleteFileAsync(imageUrl);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image");
            return BadRequest(new { success = false, message = "Failed to delete image" });
        }
    }

    private void PopulateDropdowns(BranchFormViewModel model)
    {
        var currentLang = Thread.CurrentThread.CurrentCulture.Name;
        var isArabic = currentLang.StartsWith("ar");

        model.GovernorateList = new SelectList(
            _context.Governorates.ToList(),
            "Id",
            isArabic ? "NameAr" : "NameEn",
            model.GovernorateId
        );

        if (model.GovernorateId > 0)
        {
            model.AreaList = new SelectList(
                _context.Areas.Where(a => a.GovernorateId == model.GovernorateId).ToList(),
                "Id",
                isArabic ? "NameAr" : "NameEn",
                model.AreaId
            );
        }
        else
        {
            model.AreaList = Enumerable.Empty<SelectListItem>();
        }
    }

    }
