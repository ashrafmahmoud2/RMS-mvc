namespace RMS.Web.Core.Models
{
    [Index(nameof(NameEn), IsUnique = true)]
    [Index(nameof(NameAr), IsUnique = true)]
    public class Governorate /* : BaseModel*/
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string NameEn { get; set; } = null!;

        [StringLength(100)]
        public string NameAr { get; set; } = null!;

        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}