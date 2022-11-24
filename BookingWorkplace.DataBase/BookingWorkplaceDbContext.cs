using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.DataBase;

public class BookingWorkplaceDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentForWorkplace> EquipmentForWorkplaces { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Workplace> Workplaces { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Reservation>()
            .HasIndex(reservation => new {
                reservation.WorkplaceId,
                reservation.TimeFrom,
                reservation.TimeTo,
            })
            .IsUnique();

        builder.Entity<Workplace>()
            .HasIndex(workplace => new {
                workplace.Room,
                workplace.Floor,
                workplace.DeskNumber
            })
            .IsUnique();

        builder.Entity<EquipmentForWorkplace>()
            .HasIndex(eFW => new {
                eFW.EquipmentId, WorkplacesId = eFW.WorkplaceId
            })
            .IsUnique();

        builder.Entity<Equipment>()
            .HasIndex(equipment => equipment.Id)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        builder.Entity<Role>()
            .HasIndex(role => role.Name)
            .IsUnique();
    }

    public BookingWorkplaceDbContext(DbContextOptions<BookingWorkplaceDbContext> options)
        : base(options)
    {
    }
}