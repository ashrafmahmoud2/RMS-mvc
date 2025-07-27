using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class ToppingOption
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ToppingGroupId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0;

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    [ForeignKey("ToppingGroupId")]
    public ToppingGroup ToppingGroup { get; set; } = null!;
}






