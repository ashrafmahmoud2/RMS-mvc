using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Order;

public class OrderDetailsViewModel
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    //// Current status of the order
    //public OrderStatus OrderStatus { get; set; }
    //public DateTime CreatedAt { get; set; }
    //public DateTime? DeliveredAt { get; set; }

    //// Customer info
    //public string CustomerName { get; set; } = string.Empty;
    //public string PhoneNumber { get; set; } = string.Empty;

    //// Address
    //public CustomerAddressViewModel CustomerAddress { get; set; } = new();

    //// Items
    //public List<OrderItemViewModel> Items { get; set; } = new();

    //// Totals
    //public decimal SubTotal { get; set; }
    //public decimal DeliveryFees { get; set; }
    //public decimal DiscountAmount { get; set; }
    //public decimal GrandTotal { get; set; }

    //// Payment Info
    //public PaymentSummaryViewModel Payment { get; set; } = new();
}

public class PaymentSummaryViewModel
{
    public decimal Amount { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class CustomerAddressViewModel
{
    public int GovernrateId { get; set; }
    public int AreaId { get; set; }
    public int BranchId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string BuildingDetails { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string? FlatNumber { get; set; }
    public string? Notes { get; set; }
}
