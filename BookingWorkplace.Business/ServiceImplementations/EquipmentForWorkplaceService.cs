using AutoMapper;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.Business.ServiceImplementations;

public class EquipmentForWorkplaceService : IEquipmentForWorkplaceService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReservationService _reservationService;

    public EquipmentForWorkplaceService(IMapper mapper,
        IUnitOfWork unitOfWork,
        IReservationService reservationService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _reservationService = reservationService;
    }

    /// <summary>
    ///     Gets the object that corresponding Id  from the data source and returns as a response.
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.EquipmentForWorkplaces.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentException("No record of the equipment for workplace was found in the database.",
                nameof(id));

        var dto = _mapper.Map<EquipmentForWorkplaceDto>(entity);
        return dto;
    }

    /// <summary>
    ///     Gets equipment by the equipment for the workplace unique identifier.
    /// </summary>
    /// <param name="id">an unique identifier of the equipment for workplace as a <see cref="Guid" /></param>
    /// <returns>
    ///     <see cref="EquipmentDto" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentDto> GetEquipmentByEquipmentForWorkplaceIdAsync(Guid id)
    {
        var entity = await _unitOfWork.EquipmentForWorkplaces
            .Get()
            .Include(equip => equip.Equipment)
            .AsNoTracking()
            .FirstOrDefaultAsync(equip => equip.Id.Equals(id));

        if (entity == null)
            throw new ArgumentException(nameof(id));

        return _mapper.Map<EquipmentDto>(entity.Equipment);
    }

    /// <summary>
    ///     Gets a equipment movement data.
    /// </summary>
    /// <param name="id">a unique identifier of the equipment for the workplace</param>
    /// <param name="destinationId">a unique destination identifier</param>
    /// <returns>a equipment movement data as a <see cref="EquipmentMovementData" /></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentMovementData> GetEquipmentMovementDataAsync(Guid id, Guid destinationId)
    {
        var entity = await _unitOfWork.EquipmentForWorkplaces.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentException(nameof(id));

        var dateOfMovement = await _reservationService
            .GetEndDateOfReservationByWorkplaceId(entity.WorkplaceId);

        var equipmentMovementData = new EquipmentMovementData
        {
            EquipmentId = id,
            PreviousWorkplace = entity.WorkplaceId,
            DestinationWorkplace = destinationId,
            DateOfMovement = dateOfMovement
        };

        return equipmentMovementData;
    }

    /// <summary>
    ///     Checks the possibility of forming a pool of necessary equipment to satisfy the filter list.
    /// </summary>
    /// <param name="parameters">object that implements <see cref="IFilterParameters" />.</param>
    /// <returns>A boolean</returns>
    public async Task<bool> IsPossibleToFindNecessaryEquipmentToMoveAsync(IFilterParameters parameters)
    {
        var result = new List<bool>();

        foreach (var id in parameters.EquipmentIds)
        {
            var entity = await _unitOfWork.EquipmentForWorkplaces
                .Get()
                .Where(eFW =>
                    !eFW.Workplace.Reservations
                        .Any(r => r.TimeTo >= parameters.TimeFrom))
                .Where(eFW => eFW.EquipmentId.Equals(id))
                .FirstOrDefaultAsync();

            result.Add(entity != null);
        }

        return result.TrueForAll(r => r);
    }

    public async Task<List<EquipmentForWorkplaceDto>> GetMovableEquipmentForWorkplaceAsync(IFilterParameters parameters)
    {
        var list = new List<EquipmentForWorkplaceDto>();
        foreach (var id in parameters.EquipmentIds)
        {
            var entity = await _unitOfWork.EquipmentForWorkplaces
                .Get()
                .Where(eFW =>
                    !eFW.Workplace.Reservations
                        .Any(r => r.TimeTo >= parameters.TimeFrom))
                .Where(eFW => eFW.EquipmentId.Equals(id))
                .FirstOrDefaultAsync();

            if (entity != null)
            {
                var dto = _mapper.Map<EquipmentForWorkplaceDto>(entity);
                list.Add(dto);
            }
        }

        return list;
    }

    /// <summary>
    ///     Creates a new record in the data source
    /// </summary>
    /// <param name="dto">data transfer object</param>
    /// <returns>the number of successfully created records in the data source</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> CreateEquipmentForWorkplaceAsync(EquipmentForWorkplaceDto dto)
    {
        var entity = _mapper.Map<EquipmentForWorkplace>(dto);

        if (entity == null)
            throw new ArgumentException("Mapping EquipmentForWorkplaceDto to EquipmentForWorkplace was not possible.",
                nameof(dto));

        await _unitOfWork.EquipmentForWorkplaces.AddAsync(entity);
        var result = await _unitOfWork.Commit();
        return result;
    }

    /// <summary>
    ///     Prepares records for patching.
    /// </summary>
    /// <remarks>
    ///     There is no change in the data source.
    ///     Changes will be committed at the moment of calling save changes for DBContext
    /// </remarks>
    /// <param name="equipmentIds">List of relocation equipment</param>
    /// <param name="workplaceId">workplace unique identifier as <see cref="Guid" /></param>
    /// <returns>The Task</returns>
    public async Task PrepareEquipmentForRelocationToWorkplaceAsync(List<EquipmentForWorkplaceDto> equipment,
        Guid workplaceId)
    {
        foreach (var dto in equipment)
        {
            var sourceDto = await GetEquipmentForWorkplaceByIdAsync(dto.Id);

            var patchList = new List<PatchModel>();

            if (!workplaceId.Equals(sourceDto.WorkplaceId))
                patchList.Add(new PatchModel
                {
                    PropertyName = nameof(sourceDto.WorkplaceId),
                    PropertyValue = workplaceId
                });

            await _unitOfWork.EquipmentForWorkplaces.PatchAsync(dto.Id, patchList);
        }
    }

    /// <summary>
    ///     Remove a record from the data source
    /// </summary>
    /// <param name="id">unique identifier of record</param>
    /// <returns>the number of records successfully removed</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.EquipmentForWorkplaces.GetByIdAsync(id);

        if (entity != null)
        {
            _unitOfWork.EquipmentForWorkplaces.Remove(entity);
            return await _unitOfWork.Commit();
        }

        throw new ArgumentException("The equipment for workplace for removing doesn't exist", nameof(id));
    }
}