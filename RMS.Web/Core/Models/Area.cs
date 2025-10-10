using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

[Index(nameof(NameEn), nameof(GovernorateId), IsUnique = true)]
[Index(nameof(NameAr), nameof(GovernorateId), IsUnique = true)]
public class Area /*: BaseModel*/
{
    public int Id { get; set; }

    [StringLength(100)]
    public string NameEn { get; set; } = null!;

    [StringLength(100)]
    public string NameAr { get; set; } = null!;
    public int GovernorateId { get; set; }

    public Governorate? Governorate { get; set; } = null!;

    public ICollection<Branch> Branches { get; set; } = new List<Branch>(); 
}

