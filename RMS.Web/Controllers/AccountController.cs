using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.Account;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static NuGet.Packaging.PackagingConstants;
using static System.Net.WebRequestMethods;

namespace RMS.Web.Controllers;

//stop in make docker(make by ui or gpt .net 9) to give to  mohmode push the code in digitel ocuean 
/*
# Steps
1. optmize responsive of items and container in ipad 
2. Insert real demo data for clarity in showcases , but item image in clude flair
3. Apply UI (Arabic & English):
   - Start with restaurant side:
     • Menu (items, categories, toppings) • Branches • Analytics
     • Orders  • KDS  • Customers • Settings  • Reports
*/





public class AccountController : Controller
{



    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApplicationDbContext context,
        IMapper mapper,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<AccountController> logger)
    {
        _context = context;
        _mapper = mapper;
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }


    [HttpGet]
    public IActionResult GetLoginModal()
    {
        // Return the login modal as a partial view with anti-forgery token
        return PartialView("_LoginModal");
    }

    [HttpGet]
    public IActionResult Login()
    {
        
            return PartialView("_LoginModal");
        

    }


    //[HttpGet]
    //public IActionResult Login() => View();



        [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> SendOtp(SendOtpRequest request)
    {

        try
        {
            if (!IsValidEgyptianPhone(request.PhoneNumber))
                return BadRequest(new { success = false, message = "رقم الهاتف غير صحيح" });

            //var otp = new Random().Next(0, 10000).ToString("D4");
            var otp = "9999";
            var expiry = DateTime.UtcNow.AddMinutes(5);

            HttpContext.Session.SetString(
                    $"otp_{request.PhoneNumber}",
                    $"{otp}|{expiry.Ticks}"
                );

           // var smsResult = await SendSmsViaBeOn(request.PhoneNumber, otp);
            //if (!smsResult.Success)
            //    return BadRequest(new { success = false, message = "فشل إرسال رمز التحقق" });

            return PartialView("_OtpVerification", new VerifyOtpRequest { PhoneNumber = request.PhoneNumber });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP to {Phone}", request.PhoneNumber);
            return StatusCode(500, new { success = false, message = "حدث خطأ غير متوقع" });
        }
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Otp))
                return BadRequest(new { success = false, message = "البيانات المطلوبة مفقودة" });

            var otpKey = $"otp_{request.PhoneNumber}";
            var storedData = HttpContext.Session.GetString(otpKey);
            if (string.IsNullOrEmpty(storedData))
                return BadRequest(new { success = false, message = "انتهت صلاحية الرمز" });

            var parts = storedData.Split('|');
            if (parts.Length != 2 ||
                parts[0] != request.Otp ||
                new DateTime(long.Parse(parts[1])) < DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "رمز غير صحيح أو منتهي الصلاحية" });
            }


            HttpContext.Session.Remove(otpKey);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = request.PhoneNumber,
                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    FullName = $"عميل {request.PhoneNumber[^4..]}",
                    CreatedOn = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user: {Errors}", errors);
                    return BadRequest(new { success = false, message = "فشل إنشاء المستخدم" });
                }

                if (!await _context.Customers.AnyAsync(c => c.UserId == user.Id))
                {
                    _context.Customers.Add(new Customer { UserId = user.Id, User = user });
                    await _context.SaveChangesAsync();
                }
            }

            await _signInManager.SignInAsync(user, true);

            return Ok(new { success = true, message = "تم التحقق من الرمز بنجاح" });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for {Phone}", request.PhoneNumber);
            return StatusCode(500, new { success = false, message = "حدث خطأ غير متوقع" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoSignIn([FromBody] AutoSignInRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OtpToken))
                return BadRequest(new { success = false, message = "البيانات مفقودة" });

            if (!ValidateOtpToken(request.PhoneNumber, request.OtpToken))
                return BadRequest(new { success = false, message = "رمز غير صحيح" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                return BadRequest(new { success = false, message = "المستخدم غير موجود" });

            await _signInManager.SignInAsync(user, true);
            return Ok(new { success = true, message = "تم تسجيل الدخول تلقائياً" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error auto signing in {Phone}", request.PhoneNumber);
            return StatusCode(500, new { success = false, message = "حدث خطأ غير متوقع" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }

    private async Task<SmsResult> SendSmsViaBeOn(string phoneNumber, string otp)
    {
        try
        {
            var token = _configuration["BeOn:Token"];
            var baseUrl = _configuration["BeOn:BaseUrl"];
            // should be: https://v3.api.beon.chat/api/v3/messages

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(baseUrl))
                return new SmsResult(false, "BeOn configuration missing");

            // Ensure +20 prefix
            var formattedPhone = phoneNumber.StartsWith("+20") ? phoneNumber : $"+20{phoneNumber}";

            using var form = new MultipartFormDataContent
        {
            { new StringContent(formattedPhone), "phoneNumber" },
            { new StringContent("Gust"), "name" },
            { new StringContent("sms"), "type" },
            { new StringContent("4"), "otp_length" }, // default = 4
            { new StringContent("ar"), "lang" },       // Arabic messages
            { new StringContent("123"), "reference" }, // optional
            { new StringContent(otp), "custom_code" }  // send your OTP manually
        };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("beon-token", token);

            var response = await _httpClient.PostAsync($"{baseUrl}/otp", form);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("BeOn OTP failed. Status: {Status}, Body: {Body}", response.StatusCode, body);
                return new SmsResult(false, $"HTTP {response.StatusCode}: {body}");
            }

            return new SmsResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending OTP via BeOn");
            return new SmsResult(false, ex.Message);
        }
    }

    private static bool IsValidEgyptianPhone(string phone) =>
        !string.IsNullOrWhiteSpace(phone) && Regex.IsMatch(phone, @"^01[0125][0-9]{8}$");

    private static string GenerateOtpToken(string phone)
    {
        var data = $"{phone}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    }

    private static bool ValidateOtpToken(string phone, string token)
    {
        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
            if (parts.Length != 2) return false;

            var tokenPhone = parts[0];
            var time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[1]));
            return tokenPhone == phone && DateTime.UtcNow - time < TimeSpan.FromHours(24);
        }
        catch { return false; }
    }

    private record SmsResult(bool Success, string? Error = null);
}
