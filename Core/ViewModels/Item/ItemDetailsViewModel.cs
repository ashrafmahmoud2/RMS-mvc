namespace RMS.Web.Core.ViewModels.Item;



public class ItemDetailsViewModel
{
    public int Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    
    public int LikeCount { get; set; }
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbs { get; set; }
    public string? IngredientsAr { get; set; }
    public string? IngredientsEn { get; set; }

    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? TopColor { get; set; }
    public string? BottomColor { get; set; }

    public decimal BasePrice { get; set; }




    //Allergy table
    public int AllergyId { get; set; }
    public string? AllergyNameAr { get; set; }
    public string? AllergyNameEn { get; set; }
    public string? AllergyImageUrl { get; set; }


    public ICollection<ItemToppingGroupViewModel> ItemToppingGroups { get; set; } = new List<ItemToppingGroupViewModel>();

    //public ICollection<BranchItemViewModel> BranchItems { get; set; } = new List<BranchItemViewModel>();

}

public class ItemToppingGroupViewModel
{
    public int Id { get; set; }
    public int ToppingGroupId { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public bool IsRequired { get; set; }
    public int MaxAllowedOptions { get; set; }
    public int MinAllowedOptions { get; set; }
    public int SortOrder { get; set; }

    public ICollection<ToppingOptionViewModel> ToppingOptions { get; set; } = new List<ToppingOptionViewModel>();
}

public class ToppingOptionViewModel
{
    public int Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int MaxAllowedQuantity { get; set; }
    public bool IsDefault { get; set; }
}






//public class BranchItemViewModel
//{
//    public int BranchItemId { get; set; }
//    public int BranchId { get; set; }
//    public string BranchName { get; set; } = null!;
//    public decimal BasePrice { get; set; }
//    public decimal? DiscountPercent { get; set; }
//    public decimal? PriceWithoutDiscount { get; set; }
//    public decimal? CashbackPercent { get; set; }
//    public int SalesCount { get; set; }
//    public int? Stock { get; set; }
//    public int? MaxOrderQuantity { get; set; }
//    public bool IsAvailable { get; set; }
//}
