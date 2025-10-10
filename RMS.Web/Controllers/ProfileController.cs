using Microsoft.AspNetCore.Mvc;
using RMS.Web.Core.ViewModels.Profile;

namespace RMS.Web.Controllers;
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return View("LoginRequiredView");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);



        var model = new ProfileViewModel
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            SecondaryPhoneNumber = customer?.SecondaryPhoneNumber
        };  

        return View(model);
    }

    // POST: /Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (await _userManager.Users.AnyAsync(u => u.Email == model.Email && u.Id != user.Id))
        {
            ModelState.AddModelError("Email", "هذا البريد الإلكتروني مستخدم بالفعل");
            return View(model);
        }

        if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != user.Id))
        {
            ModelState.AddModelError("PhoneNumber", "رقم الهاتف هذا مستخدم بالفعل");
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.FullName) &&
            await _userManager.Users.AnyAsync(u => u.FullName == model.FullName && u.Id != user.Id))
        {
            ModelState.AddModelError("FullName", "الاسم الكامل مستخدم بالفعل");
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.SecondaryPhoneNumber) &&
            await _context.Customers.AnyAsync(c => c.SecondaryPhoneNumber == model.SecondaryPhoneNumber && c.UserId != user.Id))
        {
            ModelState.AddModelError("SecondaryPhoneNumber", "رقم الهاتف الثانوي مستخدم بالفعل");
            return View(model);
        }

        // Update ApplicationUser
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email; 
        user.PhoneNumber = model.PhoneNumber;
        user.LastUpdatedOn = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            // Update Customer (SecondaryPhoneNumber)
            if (customer != null)
            {
                customer.SecondaryPhoneNumber = model.SecondaryPhoneNumber;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تم حفظ التعديلات بنجاح ✅";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }
}
