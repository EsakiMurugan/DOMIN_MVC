using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DOMIN_MVC.Models;

namespace DOMIN_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DOMIN_MVC.Models.Pizza>? Pizza { get; set; }
        public DbSet<DOMIN_MVC.Models.Customer>? Customer { get; set; }
        public DbSet<DOMIN_MVC.Models.Admin>? Admin { get; set; }
        public DbSet<DOMIN_MVC.Models.Cart>? Cart { get; set; }
        public DbSet<DOMIN_MVC.Models.Payment>? Payment { get; set; }
        public DbSet<DOMIN_MVC.Models.Receipt>? Receipt { get; set; }
    }
}