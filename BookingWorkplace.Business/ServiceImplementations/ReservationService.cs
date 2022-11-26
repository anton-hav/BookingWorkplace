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

    /// <summary>
    /// Gets a reservation by unique identifier from the data source.
    /// </summary>
    /// <param name="id">unique identifier as a <see cref="Guid"/></param>
    /// <returns><see cref="ReservationDto"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ReservationDto> GetReservationByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Reservations.GetByIdAsync(id);

        if (entity != null)
        {
            var dto = _mapper.Map<ReservationDto>(entity);
            return dto;
        }

        throw new ArgumentException(nameof(id));
    }

    /// <summary>
    /// Execute records search on the data source by <see cref="IQueryStringParameters"/>. Execute a sort,
    /// and skips the number equal to the product of IQueryStringParameters.CurrentPage and IQueryStringParameters.PageSize.
    /// Retrieves *IQueryStringParameters.PageSize* of the following records.
    /// </summary>
    /// <param name="parameters">object that implements <see cref="IQueryStringParameters"/></param>
    /// <returns><see cref="PagedList{T}"/> where T is <see cref="ReservationDto"/></returns>
    public PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters)
    {
        return GetReservationsByQueryStringParameters(parameters, Guid.Empty);
    }

    /// <summary>
    /// Execute records search on the data source by <see cref="IQueryStringParameters"/>. Execute a sort,
    /// and skips the number equal to the product of IQueryStringParameters.CurrentPage and IQueryStringParameters.PageSize.
    /// Retrieves *IQueryStringParameters.PageSize* of the following records.
    /// </summary>
    /// <param name="parameters">object that implements <see cref="IQueryStringParameters"/></param>
    /// <param name="userId">unique identifier of the current user</param>
    /// <returns><see cref="PagedList{T}"/> where T is <see cref="ReservationDto"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters, Guid userId)
    {
        var query = _unitOfWork.Reservations
            .Get();

        if (!userId.Equals(Guid.Empty))
        {
            query = query.Where(entity => entity.UserId.Equals(userId));
        }

        if (!String.IsNullOrEmpty(parameters.SearchString))
            query = query.Where(entity => entity.Workplace.Room.Contains(parameters.SearchString)
                                           || entity.Workplace.Floor.Contains(parameters.SearchString)
                                           || entity.Workplace.DeskNumber.Contains(parameters.SearchString)
                                           || entity.User.Email.Contains(parameters.SearchString));

        var mappedQuery = query
            .Include(entity => entity.Workplace)
            .Include(entity => entity.User)
            .AsNoTracking()
            .OrderByDescending(entity => entity.TimeFrom)
            .ThenByDescending(entity => entity.TimeTo)
            .ThenBy(entity => entity.Workplace.Floor)
            .ThenBy(entity => entity.Workplace.Room)
            .Select(entity => _mapper.Map<ReservationDto>(entity));

        if (mappedQuery == null)
            throw new ArgumentException("Failed to find records in the database that match the specified parameters. ",
                nameof(parameters));

        var list = PagedList<ReservationDto>.ToPagedList(mappedQuery, parameters);

        return list;
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

    /// <summary>
    /// Removes a record from the data source.
    /// </summary>
    /// <param name="dto"><see cref="ReservationDto"/></param>
    /// <returns>the number of successfully removed records.</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> DeleteAsync(ReservationDto dto)
    {
        var entity = _mapper.Map<Reservation>(dto);

        if (entity == null)
            throw new ArgumentException(nameof(dto));

        _unitOfWork.Reservations.Remove(entity);
        return await _unitOfWork.Commit();
    }
}