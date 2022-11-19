using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.Data.Abstractions.Repositories;
using BookingWorkplace.DataBase;
using BookingWorkplace.DataBase.Entities;

namespace BookingWorkplace.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookingWorkplaceDbContext _dbContext;
    public IRepository<Employee> Employees { get;}
    public IRepository<Equipment> Equipment { get;}
    public IRepository<EquipmentForWorkplace> EquipmentForWorkplaces { get; }
    public IRepository<Reservation> Reservations { get;}
    public IRepository<Room> Rooms { get;}
    public IRepository<Workplace> Workplaces { get;}

    public UnitOfWork(BookingWorkplaceDbContext dbContext, 
        IRepository<Employee> employees, 
        IRepository<Equipment> equipment, 
        IRepository<EquipmentForWorkplace> equipmentForWorkplaces, 
        IRepository<Reservation> reservations, 
        IRepository<Room> rooms, 
        IRepository<Workplace> workplaces)
    {
        _dbContext = dbContext;
        Employees = employees;
        Equipment = equipment;
        EquipmentForWorkplaces = equipmentForWorkplaces;
        Reservations = reservations;
        Rooms = rooms;
        Workplaces = workplaces;
    }


    public async Task<int> Commit()
    {
        return await _dbContext.SaveChangesAsync();
    }
}