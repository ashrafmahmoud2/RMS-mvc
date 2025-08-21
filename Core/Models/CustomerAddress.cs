using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

public class CustomerAddress
{
    public int Id { get; set; }

    public int CustomerId { get; set; } 

    //[StringLength(50)]
    //public string? Label { get; set; }

    public int GovernrateId { get; set; }

    public int BranchId { get; set; }

    public int AreaId { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    public string BuildingDetails { get; set; } = null!;

    [StringLength(10)]
    public string? Floor { get; set; }

    [StringLength(10)]
    public string? FlatNumber { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    public bool IsDefault { get; set; } = false;

    public Customer Customer { get; set; } = null!;
}






