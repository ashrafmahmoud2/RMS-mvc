namespace RMS.Web.Core.ViewModels.Home;

public class HomeViewModel
{
    //make the item and category sort by the sort order
    public IEnumerable<CategoryExploreBarViewModel> CategoriesExploreBar { get; set; } = Enumerable.Empty<CategoryExploreBarViewModel>();
    public IEnumerable<CategoryWithItemsViewModel> CategoriesItems { get; set; } = Enumerable.Empty<CategoryWithItemsViewModel>();
    public IEnumerable<ItemViewModel> Items { get; set; } = Enumerable.Empty<ItemViewModel>();

    public bool OrderConfirmed { get; set; }
    public int? ConfirmedOrderId { get; set; }
}

public class CategoryWithItemsViewModel
{
    public int CategoryId { get; set; }
    public string CategoryNameAr { get; set; } = null!;
    public string CategoryNameEn { get; set; } = null!;
    public string? CategoryExploreBarImage { get; set; }

    public string ItemsCardsLayout { get; set; } = null!; 

    public List<ItemViewModel> Items { get; set; } = new();
}

public class CategoryExploreBarViewModel
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
      
    public string? CategoryExploreBarImage { get; set; }
}

public class ItemViewModel
{
    public int ItemId { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int? DeliveryTime { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsAvailable { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? PriceWithoutDiscount { get; set; }
    public string? CardLabelsEn { get; set; }
    public string? CardLabelsAr { get; set; }
}