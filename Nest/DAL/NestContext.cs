using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nest.Models;

namespace Nest.DAL
{
    public class NestContext:IdentityDbContext
    {
        public NestContext(DbContextOptions<NestContext> options):base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

    }
}
