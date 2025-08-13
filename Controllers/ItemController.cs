using Azure;
using Humanizer;
using RMS.Web.Core.ViewModels.Item;

public class ItemController : Controller
{

    private readonly ILogger<ItemController> _logger;

    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    public ItemController(ILogger<ItemController> logger, ApplicationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }





    public IActionResult Detail(int itemId)
    {
        int branchId = 1;
        var item = _context.Items
     .Include(i => i.BranchItems.Where(bi => bi.BranchId == branchId))
         .ThenInclude(bi => bi.Branch)
     .Include(i => i.ItemToppingGroups)
         .ThenInclude(itg => itg.ToppingGroup)
             .ThenInclude(tg => tg.ToppingOptions)
              .Include(i => i.Allergy)
     .FirstOrDefault(i => i.Id == itemId);







        if (item is null)
            return NotFound();

        var viewModel = _mapper.Map<ItemDetailsViewModel>(item);
        return PartialView("Detail", viewModel);
    }

}




//stop in do
//Optimize animations for smoothness and performance. Refactor and optimize code for maintainability.

//Validation:

//    In ItemToppingGroup, validate MaxAllowedOptions and MinAllowedOptions.

//    In ToppingOption, if IsDefault == true, pre-check in the UI.

//    For all public int MaxAllowedQuantity { get; set; }, enforce limits in the UI.

//UI Rules:

//    If MaxAllowedOptions == 1 in topping-options, render topping-items as radio buttons.

//    If MaxAllowedOptions > 1, render topping-items as checkboxes.

//    Disable topping-options when their MaxAllowedOptions or MaxAllowedQuantity limit is reached.

//    If MinAllowedOptions is not met, disable the "Add to Cart" button.

//Additional Requirements:

//    Responsive design across mobile, tablet, and desktop.

//    Image scaling must maintain aspect ratio.

//    Ensure UI accessibility with proper ARIA attributes.

//    Use Bootstrap classes where possible for consistent styling.Optimize animations for smoothness and performance. Refactor and optimize code for maintainability.

//Validation:

//    In ItemToppingGroup, validate MaxAllowedOptions and MinAllowedOptions.

//    In ToppingOption, if IsDefault == true, pre-check in the UI.

//    For all public int MaxAllowedQuantity { get; set; }, enforce limits in the UI.

//UI Rules:

//    If MaxAllowedOptions == 1 in topping-options, render topping-items as radio buttons.

//    If MaxAllowedOptions > 1, render topping-items as checkboxes.

//    Disable topping-options when their MaxAllowedOptions or MaxAllowedQuantity limit is reached.

//    If MinAllowedOptions is not met, disable the "Add to Cart" button.

//Additional Requirements:

//    Responsive design across mobile, tablet, and desktop.

//    Image scaling must maintain aspect ratio.

//    Ensure UI accessibility with proper ARIA attributes.

//    Use Bootstrap classes where possible for consistent styling.