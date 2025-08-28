using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.ViewModels.Home;
using System.Diagnostics;

namespace RMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(bool orderConfirmed = false, int? orderId = null)
        {
            var branchId = 1;
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

                OrderConfirmed = orderConfirmed,
                ConfirmedOrderId = orderId
            };






            return View(model);
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
}

