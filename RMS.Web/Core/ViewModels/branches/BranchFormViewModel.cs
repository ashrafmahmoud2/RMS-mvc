using Microsoft.AspNetCore.Http; 
using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Core.ViewModels.Branches;

public class BranchFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "الاسم بالإنجليزية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Branch Name (English)")]
    public string NameEn { get; set; } = null!;

    [Required(ErrorMessage = "الاسم بالعربية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Branch Name (Arabic)")]
    public string NameAr { get; set; } = null!;

    [Required(ErrorMessage = "المنطقة مطلوبة")]
    [Display(Name = "Area")]
    public int AreaId { get; set; }
    public IEnumerable<SelectListItem> AreaList { get; set; } = Enumerable.Empty<SelectListItem>();

    [Required(ErrorMessage = "المحافظة مطلوبة")]
    [Display(Name = "Governorate")]
    public int GovernorateId { get; set; }
    public IEnumerable<SelectListItem> GovernorateList { get; set; } = Enumerable.Empty<SelectListItem>();

    [Required(ErrorMessage = "العنوان بالإنجليزية مطلوب")]
    [StringLength(255)]
    [Display(Name = "Address (English)")]
    public string AddressEn { get; set; } = null!;

    [Required(ErrorMessage = "العنوان بالعربية مطلوب")]
    [StringLength(255)]
    [Display(Name = "Address (Arabic)")]
    public string AddressAr { get; set; } = null!;

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    //[StringLength(20)]
    [RegularExpression(RegexPatterns.EgyptianPhoneNumber, ErrorMessage = "رقم الهاتف يجب أن يبدأ بـ 01 أو 05 ويكون 11 رقم")]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "الحد الأقصى للدفع عند الاستلام مطلوب")]
    [Range(0, 10000, ErrorMessage = "القيمة يجب أن تكون بين 0 و 1000")]
    [Display(Name = "Max Cash on Delivery")]
    public decimal MaxCashOnDeliveryAllowed { get; set; }

    [Required(ErrorMessage = "رسوم التوصيل مطلوبة")]
    [Range(0, 1000, ErrorMessage = "القيمة يجب أن تكون بين 0 و 1000")]
    [Display(Name = "Delivery Fee")]
    public decimal DeliveryFee { get; set; }

    [Required(ErrorMessage = "وقت التوصيل مطلوب")]
    [Range(1, 240, ErrorMessage = "وقت التوصيل يجب أن يكون بين 1 و 240 دقيقة")]
    [Display(Name = "Delivery Time (Minutes)")]
    public int DeliveryTimeInMinutes { get; set; }

    [Range(1, 10000, ErrorMessage = "القيمة يجب أن تكون بين 1 و 10000")]
    [Display(Name = "Max Orders Per Day")]
    public int? MaxAllowedOrdersInDay { get; set; }

    [Display(Name = "Currently Busy")]
    public bool IsBusy { get; set; } = false;

    [Display(Name = "Currently Open")]
    public bool IsOpen { get; set; } = false;

    [Display(Name = "Upload New Images")]
    public ICollection<IFormFile> NewImageFiles { get; set; } = new List<IFormFile>();

    [Display(Name = "Existing Images")]
    public ICollection<string> ExistingBranchImagePaths { get; set; } = new List<string>();

    [Display(Name = "Working Hours")]
    public List<BranchWorkingHoursFormViewModel> WorkingHours { get; set; } = new List<BranchWorkingHoursFormViewModel>();

    [Display(Name = "Exception Hours")]
    public List<BranchExceptionHoursFormViewModel> WorkingHourExceptions { get; set; } = new List<BranchExceptionHoursFormViewModel>();
}