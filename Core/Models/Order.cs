using RMS.Web.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class Order: BaseModel
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public string CustomerIdentifier { get; set; } = string.Empty;

    public int CustomerAddressId { get; set; }
    public virtual CustomerAddress CustomerAddress { get; set; } = null!;

    public int BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public decimal SubTotal { get; set; } // sum of item prices before fees/discounts
    public decimal GrandTotal { get; set; } // after discounts + delivery
    public decimal DeliveryFees { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CashbackUsedAmount { get; set; }
    public decimal CashbackPercent { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string OrderNumber { get; set; } = string.Empty; 


    //public OrderStatusEnum LastStatus { get; set; }

    public virtual ICollection<OrderStatus> StatusHistory { get; set; } = new List<OrderStatus>();
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<OrderReview> Reviews { get; set; } = new List<OrderReview>();
}
public class SelectedToppingGroup
{
    public int Id { get; set; }   // Primary Key
    public int ToppingGroupId { get; set; }
    public virtual ToppingGroup ToppingGroup { get; set; } = null!;
    public int OrderItemId { get; set; }
    public virtual OrderItem OrderItem { get; set; } = null!;
    public virtual ICollection<SelectedToppingOption> ToppingOptions { get; set; } = new List<SelectedToppingOption>();
}

public class SelectedToppingOption
{
    public int Id { get; set; }   // Primary Key
    public int ToppingOptionId { get; set; }
    public virtual ToppingOption ToppingOption { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PriceAtOrderTime { get; set; }
    public int ToppingGroupId { get; set; }
    public virtual ToppingGroup ToppingGroup { get; set; } = null!;
}


public class OrderStatus
{
    public int Id { get; set; }

    public OrderStatusEnum Status { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    [MaxLength(50)]
    public TimeSpan? StatusDuration { get; set; }
}


public class OrderReview
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? ReviewText { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [MaxLength(1000)]
    public string? AdminResponse { get; set; }
}

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    //grandTotal
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    public PaymentMethodEnum PaymentMethod { get; set; }

    public PaymentStatusEnum PaymentStatus { get; set; }

    [MaxLength(100)]
    public string? TransactionId { get; set; } // Gateway transaction ref

    [MaxLength(100)]
    public string? PaymentReference { get; set; } // Receipt or invoice number

    public DateTime PaymentDate { get; set; } = DateTime.Now;
}