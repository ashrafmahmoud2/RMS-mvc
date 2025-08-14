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
1. add fake data in local strog to show in cart
2. when make crud in item model save in local strog , and when open make him option selectd be checked and save quentity
3. add order


#ui 
- on click on  item from cart go to the item model wiht make him option selectd be checked and save quentity
- make the cart as model in ipad , pc like item modal

 */
