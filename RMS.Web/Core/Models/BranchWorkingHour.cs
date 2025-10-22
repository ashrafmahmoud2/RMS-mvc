namespace RMS.Web.Core.Models;

public class BranchWorkingHour
{
    public int Id { get; set; }


    public int? BranchId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    public TimeSpan ClosingTime { get; set; }

    // Navigation
    public Branch Branch { get; set; } = null!;
}


