using Microsoft.AspNetCore.Mvc;
using RMS.Web.Core.ViewModels.GovernateAreaBranch;


namespace RMS.Web.Controllers;
public class CartController : Controller
{

    private readonly ApplicationDbContext _context;
    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    public IActionResult Checkout()
    {
        var model = new CheckoutViewModel
        {
            Governorates = GetGovernoratesData(),
            Areas = new List<Area>(),
            Branches = new List<Branch>()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult GetGovernorates()
    {
        var data = _context.Governorates
            .Where(g => g.Areas.Any(a => _context.Branches.Any(b => b.AreaId == a.Id))) // محافظة فيها مناطق وكل منطقة فيها فروع
            .Select(g => new { g.Id, g.NameAr })
            .ToList();

        return Ok(data);
    }

    [HttpGet]
    public IActionResult GetAreas(int governorateId)
    {
        var data = _context.Areas
            .Where(a => a.GovernorateId == governorateId
                        && _context.Branches.Any(b => b.AreaId == a.Id)) // منطقة فيها فروع
            .Select(a => new { a.Id, a.NameAr, a.GovernorateId })
            .ToList();

        return Ok(data);
    }

    [HttpGet]
    public IActionResult GetBranches(int areaId)
    {
        var data = _context.Branches
            .Where(b => b.AreaId == areaId)
            .Select(b => new { b.Id, b.NameAr, b.GovernorateId, b.AreaId })
            .ToList();

        return Ok(data);
    }

    private List<Governorate> GetGovernoratesData()
    {
        return _context.Governorates
            .Where(g => g.Areas.Any(a => _context.Branches.Any(b => b.AreaId == a.Id)))
            .ToList();
    }


}



/*    
stop in make 
1.in checkout do(
2. in same item wiht deffrent toppint , make when add item check if exsikt then check if have same topping or not
3. add order



#ui 
- on click on  item from cart go to the item model wiht make him option selectd be checked and save quentity
- make the cart as model in ipad , pc like item modal
- optmize cart ui , like in githup
 */
