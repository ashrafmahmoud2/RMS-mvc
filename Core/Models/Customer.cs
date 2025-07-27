using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Web.Core.Models;

//[Index(nameof(User.FullName), nameof(User.PhoneNumber), IsUnique = true)]
public class Customer 
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public bool IsBlackListed { get; set; } = false;

    public int? DefaultAddressId { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}




