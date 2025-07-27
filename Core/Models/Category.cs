namespace RMS.Web.Core.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Icon { get; set; }

    [StringLength(255)]
    public string? PlaceholderItemImage { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int? CategorySort { get; set; }

    public string ItemsLayout { get; set; }= null!;
}






