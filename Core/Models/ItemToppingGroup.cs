using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class ItemToppingGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    [Required]
    public int ToppingGroupId { get; set; }

    [ForeignKey("ItemId")]
    public Item Item { get; set; } = null!;

    [ForeignKey("ToppingGroupId")]
    public ToppingGroup ToppingGroup { get; set; } = null!;
}






