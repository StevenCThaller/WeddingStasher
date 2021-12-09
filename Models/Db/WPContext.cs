using Microsoft.EntityFrameworkCore;
using WeddingPhotos.Models;

namespace WeddingPhotos.Models.Db
{
    public class WPContext : DbContext 
    {
        public WPContext(DbContextOptions options) : base(options) { }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<GuestBookEntry> GuestBookEntries { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ContentType> ContentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaType>()
                        .HasData(
                            new MediaType { Id = 1, Name = "Image" },
                            new MediaType { Id = 2, Name = "Video" }
                        );

            modelBuilder.Entity<Role>()
                        .HasData(
                            new Role { Id = 1, Name = "Admin" },
                            new Role { Id = 2, Name = "Weddee" },
                            new Role { Id = 3, Name = "Guest" }
                        );

            modelBuilder.Entity<ContentType>()
                        .HasData(
                            new ContentType { Id = 1, Name = "video/mp4" },
                            new ContentType { Id = 2, Name = "video/mov" },
                            new ContentType { Id = 3, Name = "video/ogg" },
                            new ContentType { Id = 4, Name = "image/png" },
                            new ContentType { Id = 5, Name = "image/jpg" },
                            new ContentType { Id = 6, Name = "image/jpeg" }
                        );
        }
    }
}