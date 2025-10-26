using RMS.Web.Core.ViewModels.Category;
using RMS.Web.Services.Interfaces;

namespace RMS.Web.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext context, IMapper mapper, ILogger<CategoryService> logger)
    {
        _context = context;
        _mapper = mapper;
        // In a real app, ILogger<CategoryService> is better than ILogger<ItemController>
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        // Order by sort column, nulls last
        return await _context.Categories
            .OrderBy(c => c.CategorySort.HasValue)
            .ThenBy(c => c.CategorySort)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> AddCategoryAsync(CategoryFormViewModel viewModel)
    {

        if (viewModel.CategorySort.HasValue)
        {
            bool isDuplicateSort = await _context.Categories
                .AnyAsync(c => c.CategorySort == viewModel.CategorySort);

            if (isDuplicateSort)
                throw new InvalidOperationException($"CategorySort '{viewModel.CategorySort}' is already in use.");
        }

        var category = _mapper.Map<Category>(viewModel);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Category '{NameEn}' (ID: {Id}) added successfully.", category.NameEn, category.Id);
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(CategoryFormViewModel viewModel)
    {
        if (!viewModel.Id.HasValue)
            throw new ArgumentException("Category ID cannot be null for update operation.");

        var existingCategory = await GetCategoryByIdAsync(viewModel.Id.Value)
            ?? throw new KeyNotFoundException($"Category with ID {viewModel.Id} not found.");

        if (viewModel.CategorySort.HasValue)
        {
            bool isDuplicateSort = await _context.Categories
                .AnyAsync(c => c.CategorySort == viewModel.CategorySort && c.Id != viewModel.Id);

            if (isDuplicateSort)
                throw new InvalidOperationException($"CategorySort '{viewModel.CategorySort}' is already in use.");
        }

        _mapper.Map(viewModel, existingCategory);
        existingCategory.ItemsCardsLayout ??= ItemLayoutType.RectangleItemLayout.ToString();

        _context.Categories.Update(existingCategory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Category '{NameEn}' (ID: {Id}) updated successfully.", existingCategory.NameEn, existingCategory.Id);
        return existingCategory;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category is null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found for deletion.");
        }

        // Note: EF Core will throw an exception if there are related Items and cascade delete is not configured.
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        _logger.LogWarning("Category '{NameEn}' (ID: {Id}) deleted successfully.", category.NameEn, category.Id);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(e => e.Id == id);
    }
}
