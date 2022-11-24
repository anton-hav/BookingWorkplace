using AutoMapper;
using BookingReservation.Core.Abstractions;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;

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

    public async Task<bool> IsReservationExistAsync(Guid workplaceId, DateTime timeFrom, DateTime timeTo)
    {
        throw new NotImplementedException();
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