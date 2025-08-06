namespace RMS.Web.Core.Models;

[Index(nameof(Name), IsUnique = true)]
public class Branch
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    public int AreaId { get; set; }

    public int GovernorateId { get; set; }

    [Required, StringLength(255)]
    public string Address { get; set; } = null!;

    [Required, StringLength(20)]
    public string Phone { get; set; } = null!;

    public decimal MaxCashOnDeliveryAllowed { get; set; }

    public string? BranchImages { get; set; }

    public bool IsBusy { get; set; } = false; //allow to bruch to close and open in exption days
    public bool IsOpen { get; set; } = false;

    public int? MaxAllowedOrdersInDay { get; set; } //if order be over the limit the branch will make IsBusy = true
    public ICollection<BranchWorkingHour> BranchWorkingHours { get; set; } = new List<BranchWorkingHour>();
    public ICollection<BranchWorkingHourException> WorkingHourExceptions { get; set; } = new List<BranchWorkingHourException>();
}


