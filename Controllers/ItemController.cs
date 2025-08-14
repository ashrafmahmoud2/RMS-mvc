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
        const int branchId = 1;

        var item = _context.Items
            .Where(i => i.Id == itemId)
            .Include(i => i.ItemToppingGroups)
                .ThenInclude(itg => itg.ToppingGroup)
                    .ThenInclude(tg => tg.ToppingOptions)
            .Include(i => i.Allergy)
            .Include(i => i.BranchItems.Where(bi => bi.BranchId == branchId))
             .ThenInclude(bi => bi.Branch)
            .FirstOrDefault();

        if (item is null || !item.BranchItems.Any() || item.BranchItems.First().BasePrice == 0m)
            return NotFound();





        var viewModel = _mapper.Map<ItemDetailsViewModel>(item);
        return PartialView("Detail", viewModel);
    }


}






