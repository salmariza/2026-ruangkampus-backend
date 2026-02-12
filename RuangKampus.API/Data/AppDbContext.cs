using Microsoft.EntityFrameworkCore;
using RuangKampus.API.Models;

namespace RuangKampus.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomBooking> RoomBookings => Set<RoomBooking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeder data awal untuk Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Lab Agile C206",
                    Location = "Gedung D4 Lantai 2",
                    Capacity = 30,
                    IsActive = true
                },
                new Room
                {
                    Id = 2,
                    Name = "Auditorium",
                    Location = "Gedung Pascasarjana Lantai 6",
                    Capacity = 500,
                    IsActive = true
                }
            );
        }
    }
}
