namespace RMS.Web.Core.ViewModels.Branches;

public class BranchWorkingHoursFormViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Day of Week")]
    public DayOfWeek DayOfWeek { get; set; }

    [Display(Name = "Working Day")]
    public bool IsWorking { get; set; } = false;

    [Required]
    [Display(Name = "Opening Time")]
    [DataType(DataType.Time)]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    [Display(Name = "Closing Time")]
    [DataType(DataType.Time)]
    public TimeSpan ClosingTime { get; set; }
}


