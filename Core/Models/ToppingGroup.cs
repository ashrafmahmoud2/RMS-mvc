using RMS.Web.Core.Enums;

namespace RMS.Web.Core.Models;

public class ToppingGroup
{
    public int Id { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public bool IsRequired { get; set; } = false;

    public ToppingGroupType Type { get; set; } = ToppingGroupType.Addon;

    public int MaxAllowedOptions { get; set; }

    public int MinAllowedOptions { get; set; }

    public int SortOrder { get; set; }

    public ICollection<ItemToppingGroup> ItemToppingGroups { get; set; } = new List<ItemToppingGroup>();

    public ICollection<ToppingOption> ToppingOptions { get; set; } = new List<ToppingOption>();
}







