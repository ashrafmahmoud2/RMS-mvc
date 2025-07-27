using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;



[Index(nameof(NameAr), IsUnique = true)]
[Index(nameof(NameEn), IsUnique = true)]
public class Item
{
    //handel items in deiffren branches
    public int Id { get; set; }

    [MaxLength(100)]
    public string? NameAr { get; set; }

    [MaxLength(100)]
    public string? NameEn { get; set; }


    [MaxLength(500)]
    public string? DescriptionAr { get; set; }

    [MaxLength(500)]
    public string? DescriptionEn { get; set; }

    // Nutrition
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbs { get; set; }
    public string? Ingredients { get; set; }


    public int? AllergyId { get; set; }


    public int CategoryId { get; set; }

    public decimal BasePrice { get; set; }

    public decimal? DiscountPercent { get; set; }

    //public decimal? DiscountFlat { get; set; }

    public int? MaxOrderQuantity { get; set; }

    public decimal? CashbackPercent { get; set; }

    public int? PurchaseCount { get; set; } = 0;

    public int LikeCount { get; set; } = 0;

    public int? DeliveryTime { get; set; }

    public bool IsAvailable { get; set; } = true;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(255)]
    public string? ThumbnailUrl { get; set; }

    [StringLength(255)]
    public string? BackgroundImageUrl { get; set; }

    [StringLength(255)]
    public string? VideoUrl { get; set; }

    [StringLength(20)]
    public string? TopColor { get; set; }

    [StringLength(20)]
    public string? BottomColor { get; set; }

    [StringLength(100)]
    public string? StatusFlags { get; set; }

    public int? SortInCategory { get; set; }

    public Allergy? Allergy { get; set; }


    public Category? Category { get; set; }

    public ICollection<ItemToppingGroup> ItemToppingGroups { get; set; } = new List<ItemToppingGroup>();
}


public class Allergy
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string NameAr { get; set; } = null!;

    [MaxLength(100)]
    public string NameEn { get; set; } = null!;

    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}




public class BranchItem
{

    //stop in this table to Branch Item and make topping item
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal Price { get; set; }     // Branch-specific price

    public bool IsAvailable { get; set; } = true;

    public int? Stock { get; set; }        // Optional: stock per branch

    public DateTime? LastUpdated { get; set; } = DateTime.Now;
}

