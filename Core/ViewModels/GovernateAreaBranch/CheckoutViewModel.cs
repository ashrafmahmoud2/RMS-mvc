namespace RMS.Web.Core.ViewModels.GovernateAreaBranch;

public class CheckoutViewModel
{
    public List<Governorate> Governorates { get; set; } = new();
    public List<Area> Areas { get; set; } = new();
    public List<Branch> Branches { get; set; } = new();
}
