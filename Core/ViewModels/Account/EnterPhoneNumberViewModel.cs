namespace RMS.Web.Core.ViewModels.Account;

public class LoginWithPhoneViewModel
{

    [RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
    public string PhoneNumber { get; set; } = null!;

}

public class VerifyOtpViewModel
{
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "رمز التحقق مطلوب")]
    [StringLength(6, MinimumLength = 4, ErrorMessage = "رمز التحقق غير صحيح")]
    public string Otp { get; set; } = null!;
}

