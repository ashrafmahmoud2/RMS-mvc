namespace RMS.Web.Core.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int ItemId { get; set; }
    public virtual Item Item { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    public decimal PriceAtOrderTime { get; set; }
    public decimal? CashbackPercent { get; set; }

    public virtual ICollection<SelectedToppingGroup> ToppingGroups { get; set; } = new List<SelectedToppingGroup>();
}
