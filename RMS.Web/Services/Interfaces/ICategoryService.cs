using RMS.Web.Core.ViewModels.Category;

namespace RMS.Web.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> AddCategoryAsync(CategoryFormViewModel viewModel);
    Task<Category> UpdateCategoryAsync(CategoryFormViewModel viewModel);
    Task DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(int id);
}