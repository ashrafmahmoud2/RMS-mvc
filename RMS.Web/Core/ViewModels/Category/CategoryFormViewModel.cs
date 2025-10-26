namespace RMS.Web.Core.ViewModels.Category;

public class CategoryFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "الاسم بالإنجليزية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Category Name (English)")]
    public string NameEn { get; set; } = null!;

    [Required(ErrorMessage = "الاسم بالعربية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Category Name (Arabic)")]
    public string NameAr { get; set; } = null!;

    [Display(Name = "Icon Path")]
    public string? Icon { get; set; }

    [Display(Name = "Placeholder Item Image Path")]
    public string? PlaceholderItemImage { get; set; }

    [Display(Name = "Explore Bar Image Path")]
    public string? CategoryExploreBarImage { get; set; }

    [Display(Name = "Is Available")]
    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Sort Order")]
    public int? CategorySort { get; set; }

    [Required(ErrorMessage = "تخطيط بطاقات الأصناف مطلوب")]
    [Display(Name = "Items Cards Layout")]
    public string ItemsCardsLayout { get; set; } = ItemLayoutType.RectangleItemLayout;
}
