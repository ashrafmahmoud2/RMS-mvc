namespace RMS.Web.Core.ViewModels.Search;

public class SearchCategoryResultViewModel
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public string Description { get; set; }
    public string DescriptionAr { get; set; }
    public List<SearchItemViewModel> Items { get; set; } = new List<SearchItemViewModel>();
}

public class SearchItemViewModel
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public string Description { get; set; }
    public string DescriptionAr { get; set; }
    public decimal BasePrice { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryNameAr { get; set; }
    public double RelevanceScore { get; set; }
}