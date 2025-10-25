using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using RMS.Web.Core.Consts;
using RMS.Web.Core.Enums;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;
using RMS.Web.Core.ViewModels.Order;
using System;
using System.Text.Json;
using System.Threading.Tasks;






public class OrderController : Controller
{



    private readonly ILogger<ItemController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    public OrderController(ILogger<ItemController> logger, ApplicationDbContext context,
        IMapper mapper, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
                .ThenInclude(c => c.User) // Assuming ApplicationUser holds customer name/phone
            .Include(o => o.Branch)
            .Include(o => o.StatusHistory) // Required to determine current status
            .OrderByDescending(o => o.CreatedOn)
            .Take(100) // Limit results for dashboard performance
            .ToListAsync();

        var viewModel = _mapper.Map<IEnumerable<OrderListViewModel>>(orders);

        return PartialView("_OrderList", viewModel);
    }

    public IActionResult OrderConfirmation(int? orderId)
    {
        if (!orderId.HasValue)
            return RedirectToAction("Index", "Home");

        var order = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Item)
            .FirstOrDefault(o => o.Id == orderId.Value);

        if (order == null)
            return RedirectToAction("Index", "Home");

        // Pass order info to view
        return View(order);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder(CheckoutViewModel model, string ItemsJson)
     {
        try
        {
            _logger.LogInformation("CreateOrder called with model: {@Model}, ItemsJson: {ItemsJson}",
                model, ItemsJson?.Substring(0, Math.Min(ItemsJson?.Length ?? 0, 200)));

            var newOrder = model.Order;

            if (!string.IsNullOrEmpty(ItemsJson))
            {
                try
                {
                    newOrder.Items = JsonSerializer.Deserialize<List<OrderItemViewModel>>(ItemsJson)!;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize ItemsJson: {ItemsJson}", ItemsJson);
                    return JsonError("خطأ في بيانات المنتجات", StatusCodes.Status400BadRequest);
                }
            }

            // Validate ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                _logger.LogWarning("ModelState invalid: {@Errors}", errors);

                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "بيانات غير صحيحة";

                return JsonError(firstError, StatusCodes.Status400BadRequest);
            }

            if (newOrder.Items == null || !newOrder.Items.Any())
                return JsonError("يجب أن يحتوي الطلب على منتجات", StatusCodes.Status400BadRequest);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Ensure customer exists
                var customer = await EnsureCustomerExistsAsync(newOrder);

                // 2. Ensure address exists
                var address = await EnsureAddressExistsAsync(customer.Id, newOrder);

                // 3. Validate branch
                var branch = await _context.Branches.FindAsync(newOrder.BranchId);
                if (branch == null)
                    return JsonError("الفرع المحدد غير موجود", StatusCodes.Status400BadRequest);

                if (branch.IsBusy)
                    return JsonError("الفرع مشغول حالياً، يرجى المحاولة لاحقاً", StatusCodes.Status400BadRequest);

                var todayOrdersCount = await _context.Orders
                    .CountAsync(o => o.BranchId == branch.Id && o.CreatedOn.Date == DateTime.UtcNow.Date);

                if (branch.MaxAllowedOrdersInDay.HasValue &&
                    todayOrdersCount >= branch.MaxAllowedOrdersInDay.Value)
                    return JsonError("تم الوصول للحد الأقصى للطلبات اليومية في هذا الفرع", StatusCodes.Status400BadRequest);

                // 4. Validate governorate / area / branch mapping
                if (!await _context.Areas.AnyAsync(a =>
                        a.Id == newOrder.AreaId && a.GovernorateId == newOrder.GovernrateId))
                    return JsonError("المنطقة والمحافظة غير متطابقتان", StatusCodes.Status400BadRequest);

                if (branch.AreaId != newOrder.AreaId || branch.GovernorateId != newOrder.GovernrateId)
                    return JsonError("الفرع المحدد لا يخدم هذه المنطقة", StatusCodes.Status400BadRequest);

                // 5. Validate items and toppings
                var orderItems = await ValidateOrderItemsAsync(newOrder.Items, newOrder.BranchId);

                // 6. Calculate totals
                decimal subTotal = orderItems.Sum(i =>
                    (i.PriceAtOrderTime * i.Quantity) +
                    i.ToppingGroups.Sum(g =>
                        g.ToppingOptions.Sum(o => o.PriceAtOrderTime * o.Quantity)
                    )
                );
                decimal deliveryFee = branch.DeliveryFee;
                decimal grandTotal = subTotal + deliveryFee;

                if (grandTotal <= 0)
                    return JsonError("إجمالي الطلب غير صحيح", StatusCodes.Status400BadRequest);

                if (grandTotal > branch.MaxCashOnDeliveryAllowed)
                    return JsonError($"يتجاوز الطلب الحد الأقصى للدفع عند الاستلام ({branch.MaxCashOnDeliveryAllowed:F2} جنيه)", StatusCodes.Status400BadRequest);

                // 7. Create order
                var order = new Order
                {
                    CustomerId = customer.Id,
                    CustomerAddressId = address.Id,
                    BranchId = branch.Id,
                    SubTotal = subTotal,
                    DeliveryFees = deliveryFee,
                    DiscountAmount = 0,
                    CashbackUsedAmount = 0,
                    GrandTotal = grandTotal,
                    CustomerIdentifier = User?.Identity?.Name ?? HttpContext.Session.Id,
                    Items = orderItems,
                    OrderNumber = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    CreatedOn = DateTime.UtcNow
                };

                var orderStatus = new OrderStatus
                {
                    Status = OrderStatusEnum.Received,
                    Timestamp = DateTime.UtcNow
                };
                order.StatusHistory.Add(orderStatus);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 8. Create payment
                var payment = new Payment
                {
                    Order = order,
                    Amount = newOrder.Payment?.Amount > 0 ? newOrder.Payment.Amount : grandTotal,
                    PaymentMethod = newOrder.Payment?.PaymentMethod ?? PaymentMethodEnum.Cash,
                    PaymentStatus = newOrder.Payment?.PaymentStatus ?? PaymentStatusEnum.Pending,
                    TransactionId = newOrder.Payment?.TransactionId,
                    PaymentReference = newOrder.Payment?.PaymentReference,
                    PaymentDate = newOrder.Payment?.PaymentDate == default
                        ? DateTime.Now
                        : newOrder.Payment.PaymentDate
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Order created successfully: OrderId={OrderId}, OrderNumber={OrderNumber}",
                    order.Id, order.OrderNumber);

                return JsonSuccess(new
                {
                    orderId = order.Id,
                    orderNumber = order.OrderNumber,
                    redirectUrl = Url.Action("Index", "Home")
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while creating order");

                // Return specific error messages for known issues
                var errorMessage = ex.Message.Contains("is not available in this branch") ||
                                   ex.Message.Contains("out of stock") ||
                                   ex.Message.Contains("Invalid topping group") ||
                                   ex.Message.Contains("Topping option")
                                       ? ex.Message
                                       : "حدث خطأ أثناء معالجة الطلب";

                return JsonError(errorMessage, StatusCodes.Status500InternalServerError);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateOrder");
            return JsonError("حدث خطأ غير متوقع", StatusCodes.Status500InternalServerError);
        }
    }


    private JsonResult JsonError(string message, int statusCode) =>
    new JsonResult(new { success = false, message })
    {
        StatusCode = statusCode,
        ContentType = "application/json"
    };


    /// <summary>
    /// Helper for success responses (always JSON).
    /// </summary>
    private JsonResult JsonSuccess(object data) =>
        new JsonResult(new { success = true, data })
        {
            StatusCode = StatusCodes.Status200OK,
            ContentType = "application/json"
        };

    private async Task<Customer> EnsureCustomerExistsAsync(CreateOrderViewModel model)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.PhoneNumber == model.PhoneNumber);

        if (customer != null)
        {

            await _signInManager.SignInAsync(customer.User, isPersistent: true);
            return customer;
        }

        if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            throw new InvalidOperationException("Phone number is required.");



        var user = new ApplicationUser
        {
            //FullName = model.FullName,
            FullName = "Gust",
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            CreatedOn = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, AppRoles.Customer);

        await _signInManager.SignInAsync(user, isPersistent: false);

        customer = new Customer { UserId = user.Id, User = user };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();


        return customer;
    }

    private async Task<CustomerAddress> EnsureAddressExistsAsync(int customerId, CreateOrderViewModel model)
    {
        var address = await _context.CustomerAddresses
            .FirstOrDefaultAsync(a =>
                a.CustomerId == customerId &&
                a.Address == model.Address &&
                a.AreaId == model.AreaId &&
                a.GovernrateId == model.GovernrateId &&
                a.BranchId == model.BranchId);

        if (address != null) return address;

        if (string.IsNullOrWhiteSpace(model.Address))
            throw new InvalidOperationException("Delivery address is required.");

        address = new CustomerAddress
        {
            CustomerId = customerId,
            GovernrateId = model.GovernrateId,
            AreaId = model.AreaId,
            BranchId = model.BranchId,
            Address = model.Address,
            BuildingDetails = model.BuildingDetails,
            Floor = model.Floor,
            FlatNumber = model.FlatNumber
        };
        _context.CustomerAddresses.Add(address);
        await _context.SaveChangesAsync();

        return address;
    }

    private async Task<List<OrderItem>> ValidateOrderItemsAsync(List<OrderItemViewModel> items, int branchId)
    {
        var itemIds = items.Select(i => i.ItemId).ToList();

        // Fetch all branch items with related Item, ItemToppingGroups, ToppingGroups, ToppingOptions
        var branchItems = await _context.BranchItems
            .Include(bi => bi.Item)
                .ThenInclude(i => i.ItemToppingGroups)
                    .ThenInclude(itg => itg.ToppingGroup)
                        .ThenInclude(tg => tg.ToppingOptions)
            .Where(bi => bi.BranchId == branchId && itemIds.Contains(bi.ItemId))
            .ToListAsync();

        var orderItems = new List<OrderItem>();

        foreach (var itemVm in items)
        {
            var branchItem = branchItems.FirstOrDefault(bi => bi.ItemId == itemVm.ItemId);
            if (branchItem == null)
                throw new Exception($"Item '{itemVm.Title}' is not available in this branch.");

            if (!branchItem.IsAvailable || (branchItem.Stock.HasValue && branchItem.Stock.Value < itemVm.Quantity))
                throw new Exception($"Item '{branchItem.Item.NameEn}' is out of stock or not enough quantity.");

            var orderItem = new OrderItem
            {
                ItemId = branchItem.ItemId,
                Quantity = itemVm.Quantity,
                PriceAtOrderTime = branchItem.PriceWithoutDiscount ?? branchItem.BasePrice,
                CashbackPercent = branchItem.CashbackPercent
            };

            // Validate selected toppings
            foreach (var selectedGroupVm in itemVm.SelectedToppingGroups)
            {
                var itemToppingGroup = branchItem.Item.ItemToppingGroups
                    .FirstOrDefault(itg => itg.ToppingGroupId == selectedGroupVm.ToppingGroupId);

                if (itemToppingGroup == null)
                    throw new Exception($"Invalid topping group '{selectedGroupVm.ToppingGroupId}' for item '{branchItem.Item.NameEn}'.");

                var selectedGroup = new SelectedToppingGroup
                {
                    ToppingGroupId = selectedGroupVm.ToppingGroupId,
                };

                foreach (var optionVm in selectedGroupVm.SelectedToppingOptions)
                {
                    var toppingOption = itemToppingGroup.ToppingGroup.ToppingOptions
                        .FirstOrDefault(to => to.Id == optionVm.ToppingOptionId);

                    if (toppingOption == null || !toppingOption.IsAvailable)
                        throw new Exception($"Topping option '{optionVm.ToppingOptionId}' is not available for '{branchItem.Item.NameEn}'.");

                    if (optionVm.Quantity > toppingOption.MaxAllowedQuantity)
                        throw new Exception($"Topping option '{toppingOption.NameEn}' exceeds max allowed quantity.");

                    selectedGroup.ToppingOptions.Add(new SelectedToppingOption
                    {
                        ToppingOptionId = optionVm.ToppingOptionId,
                        Quantity = optionVm.Quantity,
                        PriceAtOrderTime = toppingOption.Price,
                        ToppingGroupId = selectedGroupVm.ToppingGroupId
                    });
                }

                orderItem.ToppingGroups.Add(selectedGroup);
            }

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    private bool IsBranchOpen(Branch branch)
    {
        var now = DateTime.Now;
        var today = now.DayOfWeek;

        // Check branch toggle
        if (!branch.IsOpen) return false;

        // Check exception day
        //var exception = branch.WorkingHourExceptions
        //    .FirstOrDefault(e => e.Date == DateOnly.FromDateTime(now));

        //if (exception != null)
        //    return now.TimeOfDay >= exception.OpeningTime && now.TimeOfDay <= exception.ClosingTime;

        var workingHour = _context.BranchWorkingHours
            .FirstOrDefault(bw => bw.BranchId == branch.Id && bw.DayOfWeek == today);

        // Check normal working hours
        //var workingHour = branch.BranchWorkingHours
        //    .FirstOrDefault(w => w.DayOfWeek == today);
        if (workingHour == null) return false;

        return now.TimeOfDay >= workingHour.OpeningTime && now.TimeOfDay <= workingHour.ClosingTime;
    }

   
    [HttpGet]
    public IActionResult OrderDetails(int orderId)
    {
        if (orderId == 0)
            return NotFound();

        var order = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.CustomerAddress)
                .ThenInclude(ca => ca.Governrate)
            .Include(o => o.CustomerAddress)
                .ThenInclude(ca => ca.Area)
            .Include(o => o.CustomerAddress)
                .ThenInclude(ca => ca.Branch)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .Include(o => o.Items).ThenInclude(i => i.ToppingGroups)
                .ThenInclude(g => g.ToppingOptions)
                    .ThenInclude(to => to.ToppingOption)
            .Include(o => o.Payments)
            .Include(o => o.Branch)
            .FirstOrDefault(o => o.Id == orderId);



        if (order is null)
            return NotFound();

        var viewModel = _mapper.Map<OrderDetailsViewModel>(order);
        return PartialView("OrderDetails",viewModel);
    }

    [HttpGet]
    public IActionResult ChangeStatus(int orderId, OrderStatusEnum newStatus)
    {
        var order = _context.Orders
            .Include(o => o.StatusHistory)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
            return NotFound("Order not found.");

        var lastStatus = order.StatusHistory
          .OrderByDescending(s => s.Timestamp)
          .FirstOrDefault();

        var timeToComplete = lastStatus != null
            ? DateTime.UtcNow - lastStatus.Timestamp
            : (TimeSpan?)null;

        if (lastStatus != null && lastStatus.Status == newStatus)
        {
            return BadRequest("Order already has this status.");
        }

        // Add new status entry
        order.StatusHistory.Add(new OrderStatus
        {
            Status = newStatus,
            OrderId = order.Id,
            Timestamp = DateTime.UtcNow,
            StatusDuration = timeToComplete
        });

        _context.SaveChanges();

        var viewModel = _mapper.Map<OrderStatusBoxViewModel>(order);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult CustomerOrders()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return View("LoginRequiredView");

        var orders = _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.StatusHistory)
            .Where(o => o.Customer.UserId == userId)
            .OrderBy(o => o.OrderDate)
            .ToList();



        var viewModel = _mapper.Map<IEnumerable<OrderDetailsViewModel>>(orders);

        return View(viewModel);
    }


}

