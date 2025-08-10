using RMS.Web.Core.Models;

namespace RMS.Web.Core.Models;

[Index(nameof(NameAr), IsUnique = true)]
[Index(nameof(NameEn), IsUnique = true)]
public class Item
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? NameAr { get; set; }

    [MaxLength(100)]
    public string? NameEn { get; set; }

    [MaxLength(500)]
    public string? DescriptionAr { get; set; }

    [MaxLength(500)]
    public string? DescriptionEn { get; set; }

    public int LikeCount { get; set; } = 0;

    public int? DeliveryTime { get; set; }

    // Nutrition
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbs { get; set; }

    public string? IngredientsEn { get; set; }
    public string? IngredientsAr { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? AllergyId { get; set; }
    public Allergy? Allergy { get; set; }


    public string? CardLabelsEn { get; set; } 
    public string? CardLabelsAr { get; set; } 


    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    [MaxLength(255)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(255)]
    public string? BackgroundImageUrl { get; set; }

    [MaxLength(255)]
    public string? VideoUrl { get; set; }

    [MaxLength(20)]
    public string? TopColor { get; set; }

    [MaxLength(20)]
    public string? BottomColor { get; set; }

    public int? SortInCategory { get; set; }

    public ICollection<ItemToppingGroup> ItemToppingGroups { get; set; } = new List<ItemToppingGroup>();

    //public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>(); //you need to delete data then but again
}






