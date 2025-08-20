namespace RMS.Web.Core.Models;

[Index(nameof(NameEn), IsUnique = true)]
[Index(nameof(NameAr), IsUnique = true)]
public class Branch
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string NameEn { get; set; } = null!;
    public string NameAr { get; set; } = null!;


    public int AreaId { get; set; }
    public Area? Area { get; set; }

    public int GovernorateId { get; set; }
    public Governorate? Governorate { get; set; }

    [StringLength(255)]
    public string AddressEn { get; set; } = null!;

    [StringLength(255)]
    public string AddressAr { get; set; } = null!;

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


