using RMS.Web.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int CustomerAddressId { get; set; }
    public CustomerAddress CustomerAddress { get; set; } = null!;

    public int BranchId { get; set; }

    public Branch? Branch { get; set; }

    public DateTime OrderedDate { get; set; }
    public DateTime TimeFromOpenToBuy { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal FeesTotal { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal DeliveryFees { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal CashbackUsedAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CashbackPercent { get; set; }

    public OrderStatusEnum LastStatus { get; set; }

    public ICollection<OrderStatus> StatusHistory { get; set; } = new List<OrderStatus>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<OrderReview> Reviews { get; set; } = new List<OrderReview>();
}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(10,2)")]
    public decimal CurrentPrice { get; set; } // fixed typo "CuurentPrice"

    [Column(TypeName = "decimal(5,2)")]
    public decimal CashbackPercent { get; set; }

    public ICollection<OrderItemTopping> Toppings { get; set; } = new List<OrderItemTopping>();
}

public class OrderItemTopping
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public int ToppingOptionId { get; set; }
    public ToppingOption ToppingOption { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal CurrentPrice { get; set; } // fixed typo

    [Column(TypeName = "decimal(5,2)")]
    public decimal CashbackPercent { get; set; }

    public int Quantity { get; set; } = 1;
}

public class OrderStatus
{
    public int Id { get; set; }

    public OrderStatusEnum Status { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    [MaxLength(50)]
    public string? TimeToCompleteStatus { get; set; }
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