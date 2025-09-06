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

    // Working hours for display
    public List<BranchWorkingHourViewModel> WorkingHours { get; set; } = new();
}

public class BranchWorkingHourViewModel
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
}