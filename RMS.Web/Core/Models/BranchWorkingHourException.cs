namespace RMS.Web.Core.Models;

public class BranchWorkingHourException
{
    public int Id { get; set; }

    public string ExceptionNameEn { get; set; } = null!;
    public string ExceptionNameAr { get; set; } = null!;

    [Required]
    public int BranchId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    public TimeSpan ClosingTime { get; set; }

    // Navigation
    public Branch Branch { get; set; } = null!;
}


