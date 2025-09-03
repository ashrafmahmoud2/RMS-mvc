using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;

namespace RMS.Web.Core.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(PhoneNumber), IsUnique = true)]
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? FullName { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public string? CreatedById { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string? LastUpdatedById { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public Customer? Customer { get; set; }
}
