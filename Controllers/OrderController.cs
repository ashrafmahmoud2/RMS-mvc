using RMS.Web.Core.Enums;
using RMS.Web.Core.ViewModels.Order;

public class OrderController : Controller
{

   private readonly ILogger<ItemController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    public OrderController(ILogger<ItemController> logger, ApplicationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
    {

        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                return Json(new { success = false, message = "البيانات غير صحيحة", errors = errors });
            }

            // Your order creation logic here
            var orderId = Guid.NewGuid().ToString();
            return Json(new { success = true, orderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return Json(new { success = false, message = "حدث خطأ في معالجة الطلب" });
        }
    }
    //[HttpPost]
    //public IActionResult CreateOrder([FromBody] CreateOrderViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest(new { success = false, message = "البيانات غير صحيحة" });

    //    var order = new Order();

    //    //var order = new Order
    //    //{
    //    //    CustomerName = model.FullName,
    //    //    Phone = model.PhoneNumber,
    //    //    GovernrateId = model.GovernrateId,
    //    //    AreaId = model.AreaId,
    //    //    BranchId = model.BranchId,
    //    //    Address = model.Address,
    //    //    BuildingDetails = model.BuildingDetails,
    //    //    Floor = model.Floor,
    //    //    FlatNumber = model.FlatNumber,
    //    //    Notes = model.Notes,
    //    //    SubTotal = model.SubTotal,
    //    //    DeliveryFees = model.DeliveryFees,
    //    //    GrandTotal = model.GrandTotal,
    //    //    DiscountAmount = model.DiscountAmount,
    //    //    CashbackUsedAmount = model.CashbackUsedAmount,
    //    //    CreatedOn = DateTime.Now,
    //    //    Items = model.Items.Select(i => new OrderItem
    //    //    {
    //    //        ItemId = i.ItemId,
    //    //        Title = i.Title,
    //    //        Description = i.Description,
    //    //        Quantity = i.Quantity,
    //    //        ThumbnailUrl = i.ThumbnailUrl,
    //    //        PriceAtOrderTime = i.PriceAtOrderTime,
    //    //        CashbackPercent = i.CashbackPercent,
    //    //        DiscountPercent = i.DiscountPercent,
    //    //        ToppingGroups = i.SelectedToppingGroups.Select(g => new OrderToppingGroup
    //    //        {
    //    //            ToppingGroupId = g.ToppingGroupId,
    //    //            Title = g.Title,
    //    //            Options = g.SelectedToppingOptions.Select(o => new OrderToppingOption
    //    //            {
    //    //                ToppingOptionId = o.ToppingOptionId,
    //    //                Name = o.Name,
    //    //                Quantity = o.Quantity,
    //    //                PriceAtOrderTime = o.PriceAtOrderTime,
    //    //                ImageUrl = o.ImageUrl
    //    //            }).ToList()
    //    //        }).ToList()
    //    //    }).ToList(),
    //    //    Payment = new Payment
    //    //    {
    //    //        Amount = model.Payment.Amount,
    //    //        PaymentMethod = model.Payment.PaymentMethod,
    //    //        PaymentStatus = model.Payment.PaymentStatus,
    //    //        TransactionId = model.Payment.TransactionId,
    //    //        PaymentReference = model.Payment.PaymentReference,
    //    //        PaymentDate = model.Payment.PaymentDate
    //    //    }
    //    //};

    //    _context.Orders.Add(order);
    //    _context.SaveChanges();

    //    return Json(new { success = true, orderId = order.Id });
    //}



}




