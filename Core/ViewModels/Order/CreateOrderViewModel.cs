using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Order;

public class CreateOrderViewModel
{
    //[Required(ErrorMessage = Errors.RequiredField)]
    //[MaxLength(255, ErrorMessage = Errors.MaxLength)]
    public string? FullName { get; set; } 

    [Required(ErrorMessage = Errors.RequiredField)]
    [RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
    public string PhoneNumber { get; set; } = null!;

    // 🔹 Address Info
    [Required(ErrorMessage = Errors.RequiredField)]
    public int GovernrateId { get; set; }

    [Required(ErrorMessage = Errors.RequiredField)]
    public int AreaId { get; set; }

    [Required(ErrorMessage = Errors.RequiredField)]
    public int BranchId { get; set; }

    [Required(ErrorMessage = Errors.InvalidAddress)]
    [MaxLength(255, ErrorMessage = Errors.MaxLength)]
    public string Address { get; set; } = null!;

    [MaxLength(255, ErrorMessage = Errors.MaxLength)]
    public string BuildingDetails { get; set; } = null!;

    [MaxLength(30, ErrorMessage = Errors.MaxLength)]
    public string? Floor { get; set; }

    [MaxLength(30, ErrorMessage = Errors.MaxLength)]
    public string? FlatNumber { get; set; }

    [MaxLength(500, ErrorMessage = Errors.MaxLength)]
    public string? Notes { get; set; }
    
    // Financials
    public decimal SubTotal { get; set; } // sum of item prices before fees/discounts
    public decimal GrandTotal { get; set; } // after discounts + delivery    public decimal DeliveryFees { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? CashbackUsedAmount { get; set; }
    public decimal? CashbackPercent { get; set; }

    // Items
    public List<OrderItemViewModel> Items { get; set; } = new();

    public PaymentViewModel? Payment { get; set; } = new();
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

