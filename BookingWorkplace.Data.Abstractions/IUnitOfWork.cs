using BookingWorkplace.Data.Abstractions.Repositories;
using BookingWorkplace.DataBase.Entities;

namespace BookingWorkplace.Data.Abstractions;

public interface IUnitOfWork
{
    IRepository<Employee> Employees { get; }
    IRepository<Equipment> Equipment { get; }
    IRepository<EquipmentForWorkplace> EquipmentForWorkplaces { get; }
    IRepository<Reservation> Reservations { get; }
    IRepository<Workplace> Workplaces { get; }

    Task<int> Commit();
}