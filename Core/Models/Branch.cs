namespace RMS.Web.Core.Models;

[Index(nameof(Name), IsUnique = true)]
public class Branch
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public int AreaId { get; set; }

    public int GovernrateId { get; set; }

    [Required, StringLength(255)]
    public string Address { get; set; } = null!;

    [Required, StringLength(20)]
    public string Phone { get; set; } = null!;

    public string? BranchImages { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<BranchWorkingHour> BranchWorkingHours { get; set; } = new List<BranchWorkingHour>();
    public ICollection<BranchWorkingHourException> WorkingHourExceptions { get; set; } = new List<BranchWorkingHourException>();
}

public class BranchWorkingHour
{
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan OpeningTime { get; set; }

    [Required]
    public TimeSpan ClosingTime { get; set; }

    // Navigation
    public Branch Branch { get; set; } = null!;
}


public class BranchWorkingHourException
{
    public int Id { get; set; }

    public string ExceptionName { get; set; } = null!;

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


