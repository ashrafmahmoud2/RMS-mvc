using RMS.Web.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

//[Index(nameof(NameEn), nameof(Price), IsUnique = true)]
//[Index(nameof(NameAr), nameof(Price), IsUnique = true)]
public class ToppingOption
{
    public int Id { get; set; }

    public int ToppingGroupId { get; set; }
    public ToppingGroup ToppingGroup { get; set; } = null!;

    [StringLength(100)]
    public string NameEn {  get; set; } = null!;

    [StringLength(100)]
    public string NameAr { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public int MaxAllowedQuantity { get; set; }

    public bool IsDefault { get; set; } = false;

}










