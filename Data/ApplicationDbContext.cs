using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShopUnifromProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ShopUnifromProject.Models.Item> Item { get; set; } = default!;
        public DbSet<ShopUnifromProject.Models.Category> Category { get; set; } = default!;

        public DbSet<ShopUnifromProject.Models.Order> Order { get; set; } = default!;

        public DbSet<ShopUnifromProject.Models.OrderItem> OrderItem { get; set; } = default!;



        public DbSet<ShopUnifromProject.Models.Status> Status { get; set; } = default!;


        public DbSet<ShopUnifromProject.Models.Customer> Customer { get; set; } = default!;


    }
}
