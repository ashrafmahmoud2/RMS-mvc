using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class BranchItem
{
    public int ID { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? PriceWithoutDiscount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? CashbackPercent { get; set; }

    public int SalesCount { get; set; } = 0;
    public int? Stock { get; set; }
    public int? MaxOrderQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

}






