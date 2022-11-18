using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.DataBase;

public class BookingWorkplaceDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
    public BookingWorkplaceDbContext(DbContextOptions<BookingWorkplaceDbContext> options)
        : base(options)
    {
    }
}