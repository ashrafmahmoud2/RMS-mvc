namespace RMS.Web.Core.Models;

[Index(nameof(NameEn), IsUnique = true)]
[Index(nameof(NameAr), IsUnique = true)]
public class Category
{
    public int Id { get; set; }

    [ StringLength(100)]
    public string NameEn { get; set; } = null!;

    [StringLength(100)]
    public string NameAr { get; set; } = null!;

    [StringLength(255)]
    public string? Icon { get; set; }

    [StringLength(255)]
    public string? PlaceholderItemImage { get; set; }

    public string? CategoryExploreBarImage { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int? CategorySort { get; set; }


    public string ItemsCardsLayout { get; set; }= null!; /*CategoryItemsCardsLayout*/

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}






