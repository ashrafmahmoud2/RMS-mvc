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

    // Value 0: Custom, 1: ClosedAllDay, 2: Open24Hours (based on JS logic)
    public int ExceptionType { get; set; }

    [Required]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    public TimeSpan ClosingTime { get; set; }

}


