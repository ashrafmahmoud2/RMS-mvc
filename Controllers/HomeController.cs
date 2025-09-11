using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.Enums;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.Home;
using RMS.Web.Core.ViewModels.Order;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace RMS.Web.Controllers;
public class HomeController : Controller
{

    private readonly ILogger<HomeController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    private readonly SignInManager<ApplicationUser> _signInManager;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
        IMapper mapper, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _signInManager = signInManager;
    }


    public IActionResult SearchResults(string searchQuery)
    {
        var branchId = 1;

        var query = _context.Categories
            .Include(c => c.Items)
            .ThenInclude(i => i.BranchItems)
            .OrderBy(c => c.CategorySort)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(c => c.Items.Any(i =>
                i.NameEn.Contains(searchQuery) ||
                i.NameAr.Contains(searchQuery)));
        }

        var model = new HomeViewModel
        {
            CategoriesItems = query
                .Select(c => new CategoryWithItemsViewModel
                {
                    CategoryId = c.Id,
                    CategoryNameAr = c.NameAr,
                    CategoryNameEn = c.NameEn,
                    CategoryExploreBarImage = c.CategoryExploreBarImage,
                    ItemsCardsLayout = c.ItemsCardsLayout,
                    Items = c.Items
                        .Where(i => i.BranchItems.Any(bi => bi.BranchId == branchId))
                        .Where(i => string.IsNullOrEmpty(searchQuery) ||
                            i.NameEn.Contains(searchQuery) ||
                            i.NameAr.Contains(searchQuery))
                        .OrderBy(i => i.SortInCategory)
                        .Select(i => new ItemViewModel
                        {
                            ItemId = i.Id,
                            NameAr = i.NameAr,
                            NameEn = i.NameEn,
                            DescriptionAr = i.DescriptionAr,
                            DescriptionEn = i.DescriptionEn,
                            CardLabelsAr = i.CardLabelsAr,
                            CardLabelsEn = i.CardLabelsEn,
                            DeliveryTime = i.DeliveryTime,
                            ThumbnailUrl = i.ThumbnailUrl,
                            IsAvailable = i.BranchItems.Any() && i.BranchItems.First().IsAvailable,
                            BasePrice = i.BranchItems.Any() ? i.BranchItems.First().BasePrice : 0,
                            PriceWithoutDiscount = i.BranchItems.Any() ? i.BranchItems.First().PriceWithoutDiscount : null
                        }).ToList()
                })
                .ToList()
        };

        // Return the _ProductGrid partial view with the filtered model
        return PartialView("_ProductGrid", model.CategoriesItems);
    }

    public IActionResult Index(string searchQuery)
    {
        var branchId = 1;

        List<OrderStatusBoxViewModel> currentOrders = new();

        if (User.Identity?.IsAuthenticated ?? false)
        {
            currentOrders = GetCurrentOrders();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId);
            //if (customer != null && customer.DefaultBranchId.HasValue)
            //{
            //    branchId = customer.DefaultBranchId.Value;
            //}
        }

        var query = _context.Categories
            .Include(c => c.Items)
            .ThenInclude(i => i.BranchItems)
            .OrderBy(c => c.CategorySort)
            .AsQueryable();

        // Add search filtering if a query is provided
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Use EF.Functions.Like for more efficient SQL LIKE query
            query = query.Where(c => c.Items.Any(i =>
                EF.Functions.Like(i.NameEn, $"%{searchQuery}%") ||
                EF.Functions.Like(i.NameAr, $"%{searchQuery}%")));
        }

        var model = new HomeViewModel
        {
            CategoriesExploreBar = _context.Categories
    .Where(c => c.Items.Any(i => i.BranchItems.Any(bi => bi.BranchId == branchId)))
    .OrderBy(c => c.CategorySort)
    .Select(c => new CategoryExploreBarViewModel
    {
        NameEn = c.NameEn,
        NameAr = c.NameAr,
        CategoryExploreBarImage = c.CategoryExploreBarImage
    })
    .ToList(),

            CategoriesItems = query
                .Select(c => new CategoryWithItemsViewModel
                {
                    CategoryId = c.Id,
                    CategoryNameAr = c.NameAr,
                    CategoryNameEn = c.NameEn,
                    CategoryExploreBarImage = c.CategoryExploreBarImage,
                    ItemsCardsLayout = c.ItemsCardsLayout,
                    Items = c.Items
                        .Where(i => i.BranchItems.Any(bi => bi.BranchId == branchId))
                        // Add filtering for items within the category
                        .Where(i => string.IsNullOrEmpty(searchQuery) ||
                            i.NameEn.Contains(searchQuery) ||
                            i.NameAr.Contains(searchQuery))
                        .OrderBy(i => i.SortInCategory)
                        .Select(i => new ItemViewModel
                        {
                            ItemId = i.Id,
                            NameAr = i.NameAr,
                            NameEn = i.NameEn,
                            DescriptionAr = i.DescriptionAr,
                            DescriptionEn = i.DescriptionEn,
                            CardLabelsAr = i.CardLabelsAr,
                            CardLabelsEn = i.CardLabelsEn,
                            DeliveryTime = i.DeliveryTime,
                            ThumbnailUrl = i.ThumbnailUrl,
                            IsAvailable = i.BranchItems.Any() && i.BranchItems.First().IsAvailable,
                            BasePrice = i.BranchItems.Any() ? i.BranchItems.First().BasePrice : 0,
                            PriceWithoutDiscount = i.BranchItems.Any() ? i.BranchItems.First().PriceWithoutDiscount : null
                        }).ToList()
                })
                .ToList(),

            CurrentOrders = currentOrders,
            OrderConfirmed = currentOrders.Any()
        };

        return View(model);
    }
    private List<OrderStatusBoxViewModel> GetCurrentOrders()
    {

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new List<OrderStatusBoxViewModel>(); // user not logged in

        var orders = _context.Orders
            .Include(o => o.StatusHistory)
            .Include(o => o.Branch)
            .Include(o => o.Customer)
            .Where(o => o.Customer.UserId == userId)
            .ToList() // need ToList first to filter in memory based on last status
            .Where(o =>
            {
                var lastStatus = o.StatusHistory
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefault()?.Status;

                if (lastStatus == OrderStatusEnum.DriverConfirmedDelivery
          || lastStatus == OrderStatusEnum.CustomerConfirmedDelivery
          || lastStatus == OrderStatusEnum.CancelledFromCustomer
          || lastStatus == OrderStatusEnum.CancelledFromRestaurant)
                {
                    var deliveredTime = o.StatusHistory
                        .OrderByDescending(s => s.Timestamp)
                        .FirstOrDefault()?.Timestamp;

                    if (deliveredTime.HasValue &&
                        (DateTime.UtcNow - deliveredTime.Value).TotalHours >= 1)
                    {
                        return false;
                    }
                }


                return true;
            })
            .OrderByDescending(o => o.CreatedOn)
            .ToList();

        return _mapper.Map<List<OrderStatusBoxViewModel>>(orders);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statusCode = 500)
    {
        return View(new ErrorViewModel { ErrorCode = statusCode, ErrorDescription = ReasonPhrases.GetReasonPhrase(statusCode) });
    }
}


