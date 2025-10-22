namespace RMS.Web.Core.ViewModels.GovernateAreaBranch;


public class BranchIndexViewModel
{
    public List<GovernorateWithBranchesViewModel> Governorates { get; set; } = new();
}

public class GovernorateWithBranchesViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<BranchViewModel> Branches { get; set; } = new();
}

public class BranchViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? BranchImages { get; set; }
    public bool IsBusy { get; set; }
    public bool IsOpen { get; set; }
    public decimal DeliveryFee { get; set; }
    public int DeliveryTimeInMinutes { get; set; }
    public string WorkingHoursStatus { get; set; } = null!; // "مفتوح" or "مغلق"
    public string WorkingHoursText { get; set; } = null!; // "مفتوح حتى 11:30 م" or "مغلق - يفتح غداً 9:00 ص"
    public bool IsCurrentlyOpen { get; set; }
    public string GovernorateNameAr { get; set; } = null!;
    public string AreaNameAr { get; set; } = null!;

    public List<BranchWorkingHourViewModel> WorkingHours { get; set; } = new();
}

public class BranchWorkingHourViewModel
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
}

public class BranchWorkingHourExceptionViewModel
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public string ExceptionNameEn { get; set; } = null!;
    public string ExceptionNameAr { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public bool IsClosedAllDay { get; set; }
    public bool IsOpen24Hours { get; set; }

    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }

    public string Status =>
     IsClosedAllDay ? "Closed All Day" :
     IsOpen24Hours ? "Open 24 Hours" :
     $"{OpeningTime:hh\\:mm} - {ClosingTime:hh\\:mm}";
}