namespace RMS.Web.Core.Models;

public class BranchWorkingHourException:BaseModel
{
    public int Id { get; set; }

    [StringLength(100)] 
    public string ExceptionNameEn { get; set; } = null!;

    [StringLength(100)] 
    public string ExceptionNameAr { get; set; } = null!;

    [Required]
    public int BranchId { get; set; }

    public Branch Branch { get; set; } = null!;


    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    // 1. If true, the branch is closed the entire day(s) in the range. 
    // (Addresses the "close 24h" request). Opening/Closing Times are ignored.
    public bool IsClosedAllDay { get; set; } = false;

    // 2. If true (and IsClosedAllDay is false), the branch operates 24 hours 
    // (00:00 to 00:00) during this period. (Addresses the "open 24h" request).
    public bool IsOpen24Hours { get; set; } = false;
    [Required]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    public TimeSpan ClosingTime { get; set; }

}


