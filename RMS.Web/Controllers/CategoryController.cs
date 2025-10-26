using RMS.Web.Core.ViewModels.Category;
using RMS.Web.Services.Interfaces;


/*do
1. in order of catgory don't allow duplicted
2. in ui allow to chage the sort of categoy by clic on the row and moving
3. conect wiht add image service to upload images 
*/




public class CategoryController : Controller
{
    private readonly ILogger<CategoryController> _logger; 
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryController(
        ILogger<CategoryController> logger,
        ICategoryService categoryService,
        IMapper mapper)
    {
        _logger = logger;
        _categoryService = categoryService;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var viewModels = _mapper.Map<IEnumerable<CategoryFormViewModel>>(categories);
        return View(viewModels);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            await _categoryService.AddCategoryAsync(viewModel);
            TempData["SuccessMessage"] = "? ?? ????? ????? ?????.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {NameEn}.", viewModel.NameEn);
            ModelState.AddModelError("", "??? ????? ?????. ???? ?????? ?? ??? ????? ?????.");
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        var viewModel = _mapper.Map<CategoryFormViewModel>(category);
        return View("Form",viewModel);
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            await _categoryService.UpdateCategoryAsync(viewModel);
            TempData["SuccessMessage"] = "? ?? ????? ????? ?????.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {Id}.", id);
            ModelState.AddModelError("", "??? ????? ?????. ???? ?????? ?? ??? ????? ?????.");
            return View(viewModel);
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            TempData["SuccessMessage"] = "?? ??? ????? ?????.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            TempData["ErrorMessage"] = "????? ??? ?????.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with ID {Id}.", id);
            TempData["ErrorMessage"] = "??? ??? ????? ?????. ?? ???? ?????? ?????? ???? ?????.";
            return RedirectToAction(nameof(Index));
        }
    }
}


