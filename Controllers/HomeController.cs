using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.Enums;
using RMS.Web.Core.ViewModels.Home;
using RMS.Web.Core.ViewModels.Order;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace RMS.Web.Controllers;
public class HomeController : Controller
{
//    dont give thi order in homeconttroler
//(if cancell(keep it in ui 2H or he click btn that he now))
//(if arrived(keep it in ui 2H or he click btn that  arrived))
    private readonly ILogger<HomeController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }


    public IActionResult Index()
    {
        var branchId = 1;


        //if(singin)
        var currentOrders = GetCurrentOrders();

        var model = new HomeViewModel
        {
            CategoriesExploreBar = _context.Categories
    .OrderBy(c => c.CategorySort)
    .Select(c => new CategoryExploreBarViewModel
    {
        NameEn = c.NameEn,
        NameAr = c.NameAr,
        CategoryExploreBarImage = c.CategoryExploreBarImage
    })
    .ToList(),


            CategoriesItems = _context.Categories
    .Include(c => c.Items)
        .ThenInclude(i => i.BranchItems)
    .OrderBy(c => c.CategorySort)
    .Select(c => new CategoryWithItemsViewModel
    {
        CategoryId = c.Id,
        CategoryNameAr = c.NameAr,
        CategoryNameEn = c.NameEn,
        CategoryExploreBarImage = c.CategoryExploreBarImage,
        ItemsCardsLayout = c.ItemsCardsLayout,
        Items = c.Items
         .Where(i => i.BranchItems.Any(bi => bi.BranchId == branchId))
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


    ,

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


