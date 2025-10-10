using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Order;




public class OrderDetailsViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }= null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Address
    public CustomerAddressViewModel CustomerAddress { get; set; } = new();

    // Items
    public List<OrderItemViewModel> Items { get; set; } = new();

    // Totals
    public decimal SubTotal { get; set; }
    public decimal DeliveryFees { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    //Bracnch 
    public int DeliveryTimeInMinutes { get; set; }
    public string BranchPhone { get; set; } = null!;
    public string BranchName { get; set; } = null!;


    // Payment Info
    public PaymentSummaryViewModel Payment { get; set; } = new();

    public OrderStatusBoxViewModel OrderStatusBox { get; set; } = new();
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
    public string GovernrateName { get; set; } = null!;
    public int AreaId { get; set; }
    public string AreaName { get; set; } = null!;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public string Address { get; set; }= null!;
    public string BuildingDetails { get; set; }= null!;
    public string? Floor { get; set; }
    public string? FlatNumber { get; set; }
    public string? Notes { get; set; }
}


public class OrderItemViewModel
{
    public int ItemId { get; set; }
    public string Title { get; set; }= null!;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string ThumbnailUrl { get; set; }= null!;
    public decimal PriceAtOrderTime { get; set; }
    public decimal? CashbackPercent { get; set; }
    public decimal? DiscountPercent { get; set; }

    public List<SelectedToppingGroupViewModel> SelectedToppingGroups { get; set; } = new();
}

public class SelectedToppingGroupViewModel
{
    public int ToppingGroupId { get; set; }
    public string Title { get; set; } 
    public List<SelectedToppingOptionViewModel> SelectedToppingOptions { get; set; } = new();
}

public class SelectedToppingOptionViewModel
{
    public int ToppingOptionId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtOrderTime { get; set; }
    public string ImageUrl { get; set; }= null!;
    public string Name { get; set; }= null!;
}



public class OrderStatusBoxViewModel
{
    public int Id { get; set; } 

    public OrderStatusEnum LastStatus { get; set; }

    public int LastStatusId => (int)LastStatus;

    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public int DeliveryTimeInMinutes { get; set; }

}