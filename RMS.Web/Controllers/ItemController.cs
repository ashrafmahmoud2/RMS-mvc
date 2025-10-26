using Azure;
using Humanizer;
using RMS.Web.Core.ViewModels.Item;
//# when creat item form ui
//--1.when shoice price in item in BranchItems(you can ignour if it will take time)
//--2. in options and hsi group will make it from item form , in future you can make split tab for topping
//--3. in ToppingOptions you can make ToppingGroupId many to many if have same price , and if not have make another row
//--4. 

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
        const int branchId = 1;

        var item = _context.Items
            .Where(i => i.Id == itemId)
            .Include(i => i.ItemToppingGroups)
                .ThenInclude(itg => itg.ToppingGroup)
                    .ThenInclude(tg => tg.ToppingOptions)
            .Include(i => i.Allergy)
            .Include(i => i.BranchItems)
             .ThenInclude(bi => bi.Branch)
            .FirstOrDefault();

        if (item is null || !item.BranchItems.Any() || item.BranchItems.First().BasePrice == 0m)
            return NotFound();





        var viewModel = _mapper.Map<ItemDetailsViewModel>(item);
        return PartialView("Detail", viewModel);
    }


}


