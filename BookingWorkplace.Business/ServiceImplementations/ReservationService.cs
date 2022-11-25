using AutoMapper;
using BookingReservation.Core.Abstractions;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.Business.ServiceImplementations;

public class ReservationService : IReservationService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(IMapper mapper, 
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto> GetReservationByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks for existing a record in the data source that matches the parameters.
    /// </summary>
    /// <param name="workplaceId">workplace unique identifier as a <see cref="Guid"/></param>
    /// <param name="timeFrom">a check in time as a <see cref="DateTime"/></param>
    /// <param name="timeTo">a check out time as a <see cref="DateTime"/></param>
    /// <returns>A boolean (true if the record exists, or false if it does not exist)</returns>
    public async Task<bool> IsReservationExistAsync(Guid workplaceId, DateTime timeFrom, DateTime timeTo)
    {
        var entity = await _unitOfWork.Reservations
            .Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(entity =>
                entity.WorkplaceId.Equals(workplaceId)
                && entity.TimeTo.Equals(timeTo)
                && entity.TimeFrom.Equals(timeFrom));

        return entity != null;
    }

    /// <summary>
    /// Checks for existing any record in the data source with a time interval overlapping the parameters.
    /// </summary>
    /// <param name="userId">a user unique identifier as a <see cref="Guid"/></param>
    /// <param name="timeFrom">a check in time as a <see cref="DateTime"/></param>
    /// <param name="timeTo">a check out time as a <see cref="DateTime"/></param>
    /// <returns>A boolean (true if the record exists, or false if it does not exist)</returns>
    public async Task<bool> IsReservationForUserExistAsync(Guid userId, DateTime timeFrom, DateTime timeTo)
    {
        var entity = await _unitOfWork.Reservations
            .Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(entity =>
                entity.UserId.Equals(userId)
                && ((timeFrom <= entity.TimeFrom && entity.TimeFrom <= timeTo) 
                    || (timeFrom <= entity.TimeTo && entity.TimeTo <= timeTo) 
                    || (timeFrom > entity.TimeFrom && entity.TimeFrom > timeTo)));

        return entity != null;
    }

    /// <summary>
    /// Create a new record in the data source.
    /// </summary>
    /// <param name="dto"><see cref="ReservationDto"/></param>
    /// <returns>the number of successfully created records in the data source</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> CreateReservationAsync(ReservationDto dto)
    {
        var entity = _mapper.Map<Reservation>(dto);

        if (entity == null)
            throw new ArgumentException("Mapping ReservationDto to Reservation was not possible.", nameof(dto));

        await _unitOfWork.Reservations.AddAsync(entity);
        var result = await _unitOfWork.Commit();
        return result;
    }

    public async Task<int> UpdateAsync(Guid id, ReservationDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}