using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.DataBase;

public class BookingWorkplaceDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentForWorkplace> EquipmentForWorkplaces { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Workplace> Workplaces { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Reservation>()
            .HasIndex(reservation => new
            {
                // delete EmployeeId from Index
                reservation.EmployeeId,
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
                eFW.EquipmentId,
                eFW.WorkplacesId
            })
            .IsUnique();

        builder.Entity<Equipment>()
            .HasIndex(equipment => equipment.Id)
            .IsUnique();
    }

    public BookingWorkplaceDbContext(DbContextOptions<BookingWorkplaceDbContext> options)
        : base(options)
    {
    }
}