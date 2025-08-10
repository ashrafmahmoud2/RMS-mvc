using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Xml.Serialization;
namespace RMS.Web.Data;


public class ApplicationDbContext :/* IdentityDbContext*/  IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }


    // Locations
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<Area> Areas { get; set; }

    // Branches
    public DbSet<Branch> Branches { get; set; }
    public DbSet<BranchWorkingHour> BranchWorkingHours { get; set; }
    public DbSet<BranchWorkingHourException> BranchWorkingHourExceptions { get; set; }

    // Customers
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    // Items 
    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<BranchItem> BranchItems { get; set; }

    //Toppings
    public DbSet<ItemToppingGroup> ItemToppingGroups { get; set; }
    public DbSet<ToppingGroup> ToppingGroups { get; set; }
    public DbSet<ToppingOption> ToppingOptions { get; set; }


    // Orders
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OrderItemTopping> OrderItemToppings { get; set; }
    public DbSet<Payment> Payments { get; set; }






    protected override void OnModelCreating(ModelBuilder builder)
    {


        //builder.Entity<Rental>().HasQueryFilter(e => !e.IsDeleted);
        //builder.Entity<RentalCopy>().HasQueryFilter(e => !e.Rental!.IsDeleted);





        var cascadeFKs = builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;



        base.OnModelCreating(builder);
    }





}

