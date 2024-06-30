using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductsApi.Models
{
    public class EpicContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public DbSet<Product> Products { get; set; }
        public EpicContext(DbContextOptions<EpicContext> options) : base(options)
        {
        }
    }
}
