using DlmsWebApi.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace DlmsWebApi.Repository.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Author> Author { get; set; }
        public DbSet<Category> Category { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Book>()
        //        .HasIndex(b => b.Isbn)
        //        .IsUnique();
        //}

    }
}
