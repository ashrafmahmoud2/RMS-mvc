using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Branches;

public class BranchExceptionHoursFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الاستثناء بالإنجليزية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Exception Name (English)")]
    public string ExceptionNameEn { get; set; } = null!;

    [Required(ErrorMessage = "اسم الاستثناء بالعربية مطلوب")]
    [StringLength(100)]
    [Display(Name = "Exception Name (Arabic)")]
    public string ExceptionNameAr { get; set; } = null!;

    [Required(ErrorMessage = "تاريخ البداية مطلوب")]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateOnly EndDate { get; set; }

    [Required(ErrorMessage = "نوع الاستثناء مطلوب")]
    public WorkingHourExceptionType ExceptionType { get; set; }

    [Display(Name = "Opening Time")]
    [DataType(DataType.Time)]
    public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);

    [Display(Name = "Closing Time")]
    [DataType(DataType.Time)]
    public TimeSpan ClosingTime { get; set; } = new TimeSpan(22, 0, 0);

    // Custom validation
    public bool IsValid(out string errorMessage)
    {
        if (StartDate > EndDate)
        {
            errorMessage = "تاريخ البداية يجب أن يكون قبل أو يساوي تاريخ النهاية";
            return false;
        }

          if (ExceptionType == WorkingHourExceptionType.Custom)
        {
            if (OpeningTime >= ClosingTime)
            {
                errorMessage = "وقت الفتح يجب أن يكون قبل وقت الإغلاق";
                return false;
            }
        }
       

        errorMessage = string.Empty;
        return true;
    }
}