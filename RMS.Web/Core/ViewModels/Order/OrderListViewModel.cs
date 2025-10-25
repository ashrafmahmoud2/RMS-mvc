using RMS.Web.Core.Enums;

namespace RMS.Web.Core.ViewModels.Order;

// Assuming this is within RMS.Web.Core.ViewModels.Order namespace
public class OrderListViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; } 
    public string CustomerPhoneNumber { get; set; } 
    public string BranchName { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedOn { get; set; }
    public OrderStatusEnum CurrentStatus { get; set; }
    public string CurrentStatusArabic { get; set; } 
}