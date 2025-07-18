using BetaService.Models;
using Microsoft.EntityFrameworkCore;

namespace BetaService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Phone> Phones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(p => p.PhoneId);
                entity.Property(p => p.PhoneNr)
                      .IsRequired()
                      .HasMaxLength(20);
            });
        }

    }
}
