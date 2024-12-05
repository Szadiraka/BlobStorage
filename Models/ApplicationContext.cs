using Microsoft.EntityFrameworkCore;

namespace BlobStorage.Models
{
    public class ApplicationContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id=1,
                    Name = "Bob",
                    Surname = "Lisovsky",
                    Email = "example@gmail.com",
                    Phone = "+38 0456 76 676" 
                });
        }

    }
}
