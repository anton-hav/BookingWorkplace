using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.Data.Abstractions.Repositories;
using BookingWorkplace.DataBase;
using BookingWorkplace.DataBase.Entities;

namespace BookingWorkplace.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookingWorkplaceDbContext _dbContext;
    public IRepository<User> Users { get;}
    public IRepository<Role> Roles { get; }
    public IRepository<Equipment> Equipment { get;}
    public IRepository<EquipmentForWorkplace> EquipmentForWorkplaces { get; }
    public IRepository<Reservation> Reservations { get;}
    public IRepository<Workplace> Workplaces { get;}

    public UnitOfWork(BookingWorkplaceDbContext dbContext, 
        IRepository<User> users, 
        IRepository<Equipment> equipment, 
        IRepository<EquipmentForWorkplace> equipmentForWorkplaces, 
        IRepository<Reservation> reservations, 
        IRepository<Workplace> workplaces, 
        IRepository<Role> roles)
    {
        _dbContext = dbContext;
        Users = users;
        Equipment = equipment;
        EquipmentForWorkplaces = equipmentForWorkplaces;
        Reservations = reservations;
        Workplaces = workplaces;
        Roles = roles;
    }


    public async Task<int> Commit()
    {
        return await _dbContext.SaveChangesAsync();
    }
}