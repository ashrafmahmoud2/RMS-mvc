using RMS.Web.Core.ViewModels.Order;

namespace RMS.Web.Core.ViewModels.GovernateAreaBranch;

public class CheckoutViewModel
{
    public List<Governorate> Governorates { get; set; } = new();
    public List<Area> Areas { get; set; } = new();
    public List<Branch> Branches { get; set; } = new();

    public CreateOrderViewModel Order { get; set; } = new();
}
