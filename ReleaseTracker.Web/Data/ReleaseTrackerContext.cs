using Microsoft.EntityFrameworkCore;
using ReleaseTracker.Web.Models;

namespace ReleaseTracker.Web.Data
{
    public class ReleaseTrackerContext : DbContext
    {
        public ReleaseTrackerContext(DbContextOptions<ReleaseTrackerContext> options)
            : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<Release> Releases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure App entity
            modelBuilder.Entity<App>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("UK_Apps_Name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // Configure one-to-many relationship
                entity.HasMany(e => e.Releases)
                    .WithOne(e => e.App)
                    .HasForeignKey(e => e.AppId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            });

            // Configure Release entity
            modelBuilder.Entity<Release>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AppId)
                    .HasDatabaseName("IX_Releases_AppId");

                entity.HasIndex(e => e.ReleaseDate)
                    .IsDescending()
                    .HasDatabaseName("IX_Releases_ReleaseDate");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ReleaseDate)
                    .IsRequired();

                entity.Property(e => e.ReleasedBy)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ReleaseNotes)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Environment)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(255);
            });
        }
    }
}
