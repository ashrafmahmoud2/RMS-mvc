namespace RMS.Web.Core.Models;

public class BranchImage
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;

    [Required]
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;

    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

    [Range(0, 100)]
    public int DisplayOrder { get; set; }
}


