using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Web.Core.Models;
using RMS.Web.Core.ViewModels.Account;

namespace RMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(
            ApplicationDbContext context,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //send otp and show opt view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendOtp( LoginWithPhoneViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.PhoneNumber))
                return BadRequest("رقم الهاتف مطلوب.");



            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);


            Console.WriteLine($"OTP for {model.PhoneNumber}: {otp}");

            var ViewModel=new  VerifyOtpViewModel()
            {
                PhoneNumber = model.PhoneNumber,
                Otp = otp
            };

            return View("OtpVerification", ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (user == null)
            {
                // Optionally: create a new user if not exists
                user = new ApplicationUser
                {
                    UserName = model.PhoneNumber,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest("فشل إنشاء المستخدم.");
            }

            // 3. Sign in user
            await _signInManager.SignInAsync(user, isPersistent: true);


            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
