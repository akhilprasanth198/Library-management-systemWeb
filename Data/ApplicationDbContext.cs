 using Library_management_system.Models;
 using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : DbContext
{
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
            public DbSet<Book> Books { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Borrow> Borrows { get; set; }
            public DbSet<Login> Logins { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Optionally, define relationships or additional configurations
                modelBuilder.Entity<Borrow>()
                    .HasOne<Book>()
                    .WithMany()
                    .HasForeignKey(b => b.BookId);

                modelBuilder.Entity<Borrow>()
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey(b => b.UserId);

                modelBuilder.Entity<Book>().HasData
                (
                new Book
                {
                    Id = 1001,
                    Title = "The Alchemist",
                    Author = "Paulo Coelho",
                    Language = "Portuguese",
                    Category = "Novel",
                    Quantity = 1
                });

            }
}
   

