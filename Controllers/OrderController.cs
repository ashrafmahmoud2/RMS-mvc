using RMS.Web.Core.Enums;
using RMS.Web.Core.ViewModels.Order;

public class OrderController : Controller
{
    // stop in 
    //1. Add المنطقة, أقرب فرع in lcoalstroge
  
   
       
      


    private readonly ILogger<ItemController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(ILogger<ItemController> logger, ApplicationDbContext context,
        IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
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
            return BadRequest(ModelState);

        if (model.Items == null || !model.Items.Any())
            return BadRequest("Order must contain at least one item.");

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
                return BadRequest("Branch not found.");

            //if (branch == null || !IsBranchOpen(branch))
            //    return BadRequest("Branch is closed or invalid.");


            if (branch.IsBusy)
                return BadRequest("Branch is currently busy, please try another branch.");

            var todayOrdersCount = await _context.Orders
                .CountAsync(o => o.BranchId == branch.Id && o.CreatedOn.Date == DateTime.UtcNow.Date);

            if (branch.MaxAllowedOrdersInDay.HasValue &&
                todayOrdersCount >= branch.MaxAllowedOrdersInDay.Value)
                return BadRequest("Branch has reached its daily order limit.");

            // 4. Validate governorate / area / branch mapping
            if (!await _context.Areas.AnyAsync(a => a.Id == model.AreaId && a.GovernorateId == model.GovernrateId))
                return BadRequest("Invalid governorate/area combination.");

            if (branch.AreaId != model.AreaId || branch.GovernorateId != model.GovernrateId)
                return BadRequest("Branch does not belong to the selected area/governorate.");

            // 5. Validate items and toppings
            var orderItems = await ValidateOrderItemsAsync(model.Items, model.BranchId);

            // 6. Calculate totals
            decimal subTotal = orderItems.Sum(i => i.PriceAtOrderTime * i.Quantity);
            decimal deliveryFee = branch.DeliveryFee;
            decimal grandTotal = subTotal + deliveryFee;

            if (grandTotal <= 0)
                return BadRequest("Invalid order total.");

            if (grandTotal > branch.MaxCashOnDeliveryAllowed)
                return BadRequest($"Order exceeds branch COD limit ({branch.MaxCashOnDeliveryAllowed}).");

            // 7. Create order
            var order = new Order
            {
                CustomerId = customer.Id,
                CustomerAddressId = address.Id,
                BranchId = branch.Id,
                SubTotal = subTotal,
                DeliveryFees = deliveryFee,
                DiscountAmount = 0, // TODO: apply discount logic if needed
                CashbackUsedAmount = 0,
                GrandTotal = grandTotal,
                LastStatus = OrderStatusEnum.Pending,
                Items = orderItems,
                OrderNumber = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                CreatedOn = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { success = true, orderId = order.Id, orderNumber = order.OrderNumber });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error while creating order");
            return StatusCode(500, new { success = false, message = "An error occurred while creating the order" });
        }
    }


    private async Task<Customer> EnsureCustomerExistsAsync(CreateOrderViewModel model)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.PhoneNumber == model.PhoneNumber);

        if (customer != null) return customer;

        if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            throw new InvalidOperationException("Phone number is required.");

        var user = new ApplicationUser
        {
            //FullName = model.FullName,
            FullName = "Ashrf from C#",
            PhoneNumber = model.PhoneNumber,
            UserName = model.PhoneNumber,
            CreatedOn = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unable to create user.");

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
                throw new Exception($"Item '{itemVm.ItemId}' is not available in this branch.");

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

}



