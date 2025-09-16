namespace RMS.Web.Core.ViewModels.Account;


//public class SendOtpRequest
//{
//    [RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
//    public string PhoneNumber { get; set; }
//}

//public class VerifyOtpRequest
//{
//    [RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
//    public string PhoneNumber { get; set; }
//    public string Otp { get; set; }
//}

//public class AutoSignInRequest
//{
//    [RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
//    public string PhoneNumber { get; set; }
//    public string OtpToken { get; set; }
//}

public class SendOtpRequest
{
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف يجب أن يبدأ بـ 010 أو 011 أو 012 أو 015 ويكون 11 رقم")]
    public string PhoneNumber { get; set; }
}

public class VerifyOtpRequest
{
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف غير صحيح")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "رمز التحقق مطلوب")]
    [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "رمز التحقق يجب أن يكون 6 أرقام")]
    public string Otp { get; set; }
}

public class AutoSignInRequest
{
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "رمز التحقق مطلوب")]
    public string OtpToken { get; set; }
}

public class LoginWithPhoneViewModel
{
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف يجب أن يبدأ بـ 010 أو 011 أو 012 أو 015 ويكون 11 رقم")]
    [Display(Name = "رقم الهاتف")]
    public string PhoneNumber { get; set; }
}

// Response classes for API responses
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class OtpVerificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string OtpToken { get; set; }
    public string RedirectUrl { get; set; }
}

public class SendOtpResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string PhoneNumber { get; set; }
}

