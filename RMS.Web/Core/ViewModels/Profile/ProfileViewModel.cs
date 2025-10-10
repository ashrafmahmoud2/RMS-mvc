namespace RMS.Web.Core.ViewModels.Profile;

public class ProfileViewModel
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string PhoneNumber { get; set; } = null!;

    [Phone]
    public string? SecondaryPhoneNumber { get; set; }
}
