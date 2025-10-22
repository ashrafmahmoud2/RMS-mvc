namespace RMS.Web.Core.Models;

[Index(nameof(NameEn), IsUnique = true)]
[Index(nameof(NameAr), IsUnique = true)]
public class Branch:BaseModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string NameEn { get; set; } = null!;

    [Required, StringLength(100)]
    public string NameAr { get; set; } = null!;


    public int AreaId { get; set; }
    public Area? Area { get; set; } = null!;

    public int GovernorateId { get; set; }
    public Governorate? Governorate { get; set; } = null!;

    [StringLength(255)]
    public string AddressEn { get; set; } = null!;

    [StringLength(255)]
    public string AddressAr { get; set; } = null!;

    [Required, StringLength(20)]
    public string Phone { get; set; } = null!;

    public decimal MaxCashOnDeliveryAllowed { get; set; }

    public ICollection<BranchImage> BranchImages { get; set; } = new List<BranchImage>();

    public bool IsBusy { get; set; } = false; //allow to bruch to close and open in exption days
    
    public bool IsOpen { get; set; } = false;
  //  public bool IsDefaultBranch { get; set; } = false;

    public decimal DeliveryFee { get; set; }

    [Range(1, 240)] // e.g. 1 to 240 minutes
    public int DeliveryTimeInMinutes { get; set; }

    public int? MaxAllowedOrdersInDay { get; set; } //if order be over the limit the branch will make IsBusy = true

    public ICollection<BranchWorkingHour> BranchWorkingHours { get; set; } = new List<BranchWorkingHour>();
    public ICollection<BranchWorkingHourException> WorkingHourExceptions { get; set; } = new List<BranchWorkingHourException>();
}


