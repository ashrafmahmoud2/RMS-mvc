using RMS.Web.Core.Enums;

namespace RMS.Web.Core.Models;


//add payment and Delivery table , CancelReason table 
public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int CustomerAddressId { get; set; }
    public CustomerAddress CustomerAddress { get; set; } = null!;

    public DateTime OrderedDate { get; set; }
    public DateTime TimeFromOpenToBuy { get; set; }

    public decimal FeesTotal { get; set; }
    public decimal DeliveryFees { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CashbackUsedAmount { get; set; }

    public string LastStatus { get; set; } = null!;

    public int OrderStatusId { get; set; }

    public OrderStatus OrderStatus { get; set; } 

    // public OrderSource OrderSource { get; set; }
    // public int? KioskId { get; set; }
    // public Kiosk? Kiosk { get; set; }
}

public class OrderStatus
{
    public int Id { get; set; }

    public OrderStatusEnum Status { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public string? TimeToCompleteStatus { get; set; }
}
