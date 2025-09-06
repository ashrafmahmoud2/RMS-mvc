using Microsoft.Identity.Client;
using RMS.Web.Core.Consts;
using RMS.Web.Core.Enums;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.Order;
using System;
using System.Threading.Tasks;



//When I click on

//```html
//<a href="/Support" class= "menu-item" >
//    < i class= "fa-solid fa-life-ring" ></ i >
//    < span class= "menu-item-text" > اتصل بالفرع </ span >
//    < i class= "fa-solid fa-chevron-left menu-item-arrow" ></ i >
//</ a >
//```

//I want it to behave like a **collapsible menu** instead of redirecting.

//* On click, it should expand and show two sub-options:

//  * 📞 **اتصال * * → `tel: @branch.PhoneNumber`
//  * 💬 **واتساب * * → `https://wa.me/@branch.PhoneNumber`

//*Clicking again should collapse/hide the options.

//* Smooth animation for expand/collapse.

//* Keep the main menu item highlighted/active when expanded.




/*

 # seps

 6. make regestriation to show his orders(curent or past )
 7. Implement search , using pakage in mvc project
 8. add the card layout , with make responsive in it
 9. make one modal to show error in chekout page   
 10. Make prail view if no itsm , orders to show in chekout , my orders page and if not login like orders , accout any thing need login 
    now you are ready to test in real life , so using local host and ngrok
*/

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = Errors.RequiredField });

        if (model.Items == null || !model.Items.Any())
            return BadRequest(new { success = false, message = Errors.OrderMustContainItems });

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            // 1. Ensure customer exists
            var customer = await EnsureCustomerExistsAsync(model);

            // 2. Ensure address exists
            var address = await EnsureAddressExistsAsync(customer.Id, model);

            // 3. Validate branch
            var branch = await _context.Branches.FindAsync(model.BranchId);
            if (branch == null)
                return BadRequest(new { success = false, message = Errors.BranchNotFound });

            if (branch.IsBusy)
                return BadRequest(new { success = false, message = Errors.BranchBusy });

            var todayOrdersCount = await _context.Orders
                .CountAsync(o => o.BranchId == branch.Id && o.CreatedOn.Date == DateTime.UtcNow.Date);

            if (branch.MaxAllowedOrdersInDay.HasValue &&
                todayOrdersCount >= branch.MaxAllowedOrdersInDay.Value)
                return BadRequest(new { success = false, message = Errors.BranchDailyLimitReached });

            // 4. Validate governorate / area / branch mapping
            if (!await _context.Areas.AnyAsync(a => a.Id == model.AreaId && a.GovernorateId == model.GovernrateId))
                return BadRequest(new { success = false, message = Errors.InvalidAreaGovernorate });

            if (branch.AreaId != model.AreaId || branch.GovernorateId != model.GovernrateId)
                return BadRequest(new { success = false, message = Errors.BranchAreaGovernorateMismatch });

            // 5. Validate items and toppings
            var orderItems = await ValidateOrderItemsAsync(model.Items, model.BranchId);

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
                return BadRequest(new { success = false, message = Errors.OrderInvalidTotal });

            if (grandTotal > branch.MaxCashOnDeliveryAllowed)
                return BadRequest(new { success = false, message = Errors.OrderExceedsCODLimit });

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
                //LastStatus = OrderStatusEnum.Received, 
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


            // 8. Create payment
            var payment = new Payment
            {
                Order = order,
                Amount = model.Payment.Amount > 0 ? model.Payment.Amount : grandTotal, // fallback
                PaymentMethod = model.Payment.PaymentMethod,
                PaymentStatus = model.Payment.PaymentStatus,
                TransactionId = model.Payment.TransactionId,
                PaymentReference = model.Payment.PaymentReference,
                PaymentDate = model.Payment.PaymentDate == default ? DateTime.Now : model.Payment.PaymentDate
            };



            order.StatusHistory.Add(orderStatus);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { success = true, orderId = order.Id, orderNumber = order.OrderNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            if (ex.Message.Contains("is not available in this branch") ||
          ex.Message.Contains("out of stock") ||
          ex.Message.Contains("Invalid topping group") ||
          ex.Message.Contains("Topping option"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }


            _logger.LogError(ex, "Error while creating order");
            return StatusCode(500, new { success = false, message = Errors.UnexpectedError });
        }
    }


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
            FullName = "Ashrf from C#",
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            CreatedOn = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

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
        var exception = branch.WorkingHourExceptions
            .FirstOrDefault(e => e.Date == DateOnly.FromDateTime(now));
        if (exception != null)
            return now.TimeOfDay >= exception.OpeningTime && now.TimeOfDay <= exception.ClosingTime;

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
        return View(viewModel);
    }


    [HttpGet] 
    public IActionResult ChangeStatus(int orderId, OrderStatusEnum newStatus )
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

        return RedirectToAction("Index", "Home") ;
    }

    [HttpGet]
    public IActionResult CustomerOrders()  
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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

