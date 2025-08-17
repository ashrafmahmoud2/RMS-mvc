using Microsoft.AspNetCore.Mvc;

namespace RMS.Web.Controllers;
public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}


/*
stop in make 
1. fix _CartAccessBtn.cshtml
2. add item to cart wiht all crud
3. add order


#ui 
- on click on  item from cart go to the item model wiht make him option selectd be checked and save quentity
- make the cart as model in ipad , pc like item modal

 */
