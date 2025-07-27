namespace RMS.Web.Core.Models;

public class ToppingGroup
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = null!;

    public bool IsRequired { get; set; } = false;

    public int MaxSelection { get; set; } = 1;

    public int SortOrder { get; set; } = 0;

    public ICollection<ItemToppingGroup> ItemToppingGroups { get; set; } = new List<ItemToppingGroup>();

    public ICollection<ToppingOption> ToppingOptions { get; set; } = new List<ToppingOption>();
}






