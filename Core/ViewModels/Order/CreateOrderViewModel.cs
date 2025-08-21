using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Order;

public class CreateOrderViewModel
{
    // Customer Details
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    // Address Info
    public int GovernrateId { get; set; }
    public int AreaId { get; set; }
    public int BranchId { get; set; }
    public string? Address { get; set; }
    public string BuildingDetails { get; set; } = null!;
    public string? Floor { get; set; }
    public string? FlatNumber { get; set; }
    public string? Notes { get; set; }

    // Financials
    public decimal SubTotal { get; set; } // sum of item prices before fees/discounts
    public decimal GrandTotal { get; set; } // after discounts + delivery    public decimal DeliveryFees { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CashbackUsedAmount { get; set; }
    public decimal CashbackPercent { get; set; }

    // Items
    public List<OrderItemViewModel> Items { get; set; } = new();

    public PaymentViewModel Payment { get; set; } = new();
}

public class PaymentViewModel
{
    public decimal Amount { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}

public class OrderItemViewModel
{
    public int ItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal PriceAtOrderTime { get; set; }
    public decimal? CashbackPercent { get; set; }
    public decimal? DiscountPercent { get; set; }

    public List<SelectedToppingGroupViewModel> SelectedToppingGroups { get; set; } = new();
}

public class SelectedToppingGroupViewModel
{
    public int ToppingGroupId { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<SelectedToppingOptionViewModel> SelectedToppingOptions { get; set; } = new();
}

public class SelectedToppingOptionViewModel
{
    public int ToppingOptionId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtOrderTime { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
