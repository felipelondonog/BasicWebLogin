using Microsoft.EntityFrameworkCore;

namespace BasicWebLogin.Models
{
    public class LoginDBContext : DbContext
    {
        public LoginDBContext(DbContextOptions<LoginDBContext> options) : base(options)
        {
            
        }

        public DbSet<UserModel> UserModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().ToTable("UserModel");
        }
    }
}
