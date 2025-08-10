namespace RMS.Web.Core.Models;


public class ItemToppingGroup
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public Item Item { get; set; } = null!;


    public int ToppingGroupId { get; set; }

    public ToppingGroup ToppingGroup { get; set; } = null!;
}

 






