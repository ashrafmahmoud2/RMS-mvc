namespace RMS.Web.Core.Models;

public class BranchItem
{
    public int ID { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal BasePrice { get; set; }
    public decimal Price { get; set; }

    public decimal? DiscountPercent { get; set; }
    public decimal? CashbackPercent { get; set; }

    public int SalesCount { get; set; } = 0;
    public int? Stock { get; set; }
    public int? MaxOrderQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime? LastUpdated { get; set; } = DateTime.Now;
}






