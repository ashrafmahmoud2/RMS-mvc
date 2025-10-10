using RMS.Web.Core.Enums;

namespace RMS.Web.Core.Models;

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




