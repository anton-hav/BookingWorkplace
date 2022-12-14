using AutoMapper;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookingWorkplace.Business.ServiceImplementations;

public class WorkplaceService : IWorkplaceService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public WorkplaceService(IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///     Gets the object that corresponding Id  from the data source and returns as a response.
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns>data transfer object corresponding to the id</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<WorkplaceDto> GetWorkplaceByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Workplaces.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentException("No record of the workplace was found in the database.", nameof(id));

        var dto = _mapper.Map<WorkplaceDto>(entity);
        return dto;
    }

    /// <summary>
    ///     Executes a retrieval query with the corresponding "id" of the entity in the data source include
    ///     EquipmentForWorkplaces property with its navigate properties. If a matching entity is found,
    ///     a mapping is performed from entity type to data transfer object type.
    ///     The result of the mapping is returned
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns>data transfer object corresponding to the id</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<WorkplaceDto> GetWorkplaceWithEquipmentByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Workplaces
            .Get()
            .Where(entity => entity.Id.Equals(id))
            .Include(entity => entity.EquipmentForWorkplaces)
            .ThenInclude(eFW => eFW.Equipment)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new ArgumentException("No record of the workplace was found in the database.", nameof(id));

        var dto = _mapper.Map<WorkplaceDto>(entity);
        return dto;
    }

    /// <summary>
    ///     Execute an entity search on the data source by IQueryStringParameters.SearchString. Execute a sort,
    ///     and skips the number equal to the product of IQueryStringParameters.CurrentPage and
    ///     IQueryStringParameters.PageSize.
    ///     Retrieves *IQueryStringParameters.PageSize* of the following records. Execute mapping.
    /// </summary>
    /// <param name="parameters">object that implements IQueryStringParameters</param>
    /// <returns>PagedList of data transfer objects</returns>
    public PagedList<WorkplaceDto> GetWorkplacesByQueryStringParameters(IQueryStringParameters parameters)
    {
        var query = _unitOfWork.Workplaces
            .Get()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(parameters.SearchString))
            query = query.Where(entity => entity.Room.Contains(parameters.SearchString)
                                          || entity.Floor.Contains(parameters.SearchString)
                                          || entity.DeskNumber.Contains(parameters.SearchString));

        var mappedQuery = query
            .OrderBy(entity => entity.Floor)
            .ThenBy(entity => entity.Room)
            .ThenBy(entity => entity.DeskNumber)
            .Select(entity => _mapper.Map<WorkplaceDto>(entity));

        if (mappedQuery == null)
            throw new ArgumentException("Failed to find records in the database that match the specified parameters. ",
                nameof(parameters));

        var list = PagedList<WorkplaceDto>.ToPagedList(mappedQuery, parameters);

        return list;
    }

    /// <summary>
    ///     Gets workplaces.
    /// </summary>
    /// <param name="parameters">a filter as <see cref="IFilterParameters" /></param>
    /// <returns><see cref="List{T}" /> where T is <see cref="WorkplaceDto" /></returns>
    public List<WorkplaceDto> GetWorkplacesByFilterParameters(IFilterParameters parameters)
    {
        var query = _unitOfWork.Workplaces
            .Get();

        query = query.Where(w =>
            !w.Reservations
                .Any(res => (parameters.TimeFrom <= res.TimeFrom
                             && res.TimeFrom <= parameters.TimeTo)
                            || (parameters.TimeFrom <= res.TimeTo
                                && res.TimeTo <= parameters.TimeTo)
                            || (parameters.TimeFrom > res.TimeFrom
                                && parameters.TimeTo < res.TimeTo)));

        foreach (var param in parameters.EquipmentIds)
            query = query.Where(w => w.EquipmentForWorkplaces.Any(eq => eq.EquipmentId.Equals(param)));

        var dto = query.AsNoTracking().Select(w => _mapper.Map<WorkplaceDto>(w)).ToList();
        return dto;
    }

    /// <summary>
    /// Gets a list of missing equipment for the current workplace
    /// </summary>
    /// <param name="id">a unique identifier of reserved workplace as <see cref="Guid"/></param>
    /// <param name="equipmentIds">list of equipment selected by the user</param>
    /// <returns><see cref="List{T}"/> where T is <see cref="Guid"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<List<Guid>> GetListOfMissingEquipment(Guid id, List<Guid> equipmentIds)
    {
        var entity = await _unitOfWork.Workplaces
            .Get()
            .Where(w => w.Id.Equals(id))
            .Include(w => w.EquipmentForWorkplaces)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new ArgumentException(nameof(id));

        var staffedEquipment = entity.EquipmentForWorkplaces
            .Select(equip => equip.EquipmentId)
            .OrderBy(equip => equip)
            .ToList();

        var result = equipmentIds.Except(staffedEquipment).ToList();

        return result;
    }

    /// <summary>
    ///     Gets the list of <see cref="WorkplaceDto" /> that only meet the requirements of the reservation time parameters.
    ///     Excludes from the list the <see cref="WorkplaceDto" /> contained in <see cref="exclusionList" />
    /// </summary>
    /// <param name="parameters">object that implements <see cref="IFilterParameters" />.</param>
    /// <param name="exclusionList"></param>
    /// <returns></returns>
    public async Task<List<WorkplaceDto>> GetPossibleWorkplacesByFilterParameters(IFilterParameters parameters,
        List<WorkplaceDto> exclusionList)
    {
        var query = _unitOfWork.Workplaces.Get();

        // Skip relevant workplaces
        foreach (var workplace in exclusionList) query = query.Where(w => !w.Id.Equals(workplace.Id));

        // Find free workplaces for filter parameters
        query = query.Where(w =>
            !w.Reservations
                .Any(res => (parameters.TimeFrom <= res.TimeFrom
                             && res.TimeFrom <= parameters.TimeTo)
                            || (parameters.TimeFrom <= res.TimeTo
                                && res.TimeTo <= parameters.TimeTo)
                            || (parameters.TimeFrom > res.TimeFrom
                                && parameters.TimeTo < res.TimeTo)));

        var freeWorkplaces = await query.AsNoTracking().ToListAsync();

        return _mapper.Map<List<WorkplaceDto>>(freeWorkplaces);
    }

    /// <summary>
    ///     Checks for existing a record in the data source that matches the parameters.
    /// </summary>
    /// <param name="roomNumber">room number of the current workplace as a string</param>
    /// <param name="floorNumber">floor number of the current workplace as a string</param>
    /// <param name="deskNumber">desk number of the current workplace as a string</param>
    /// <returns>A boolean (true if the record exists, or false if it does not exist)</returns>
    public async Task<bool> IsWorkplaceExistAsync(string roomNumber, string floorNumber, string deskNumber)
    {
        var entity = await _unitOfWork.Workplaces
            .Get()
            .FirstOrDefaultAsync(entity => entity.Floor.Equals(floorNumber)
                                           && entity.Room.Equals(roomNumber)
                                           && entity.DeskNumber.Equals(deskNumber));

        return entity != null;
    }

    /// <summary>
    ///     Create a new record in the data source.
    /// </summary>
    /// <param name="dto">
    ///     <see cref="WorkplaceDto" />
    /// </param>
    /// <returns>the number of successfully created records in the data source</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> CreateWorkplaceAsync(WorkplaceDto dto)
    {
        var entity = _mapper.Map<Workplace>(dto);

        if (entity == null)
            throw new ArgumentException("Mapping WorkplaceDto to Workplace was not possible.", nameof(dto));

        await _unitOfWork.Workplaces.AddAsync(entity);
        var result = await _unitOfWork.Commit();
        return result;
    }

    /// <summary>
    ///     Executes record patching in the data source
    /// </summary>
    /// <param name="id">unique identifier of record</param>
    /// <param name="dto">modified data transfer object</param>
    /// <returns>the number of records successfully changed</returns>
    public async Task<int> UpdateAsync(Guid id, WorkplaceDto dto)
    {
        var sourceDto = await GetWorkplaceByIdAsync(id);

        var patchList = new List<PatchModel>();

        if (!dto.Room.Equals(sourceDto.Room))
            patchList.Add(new PatchModel
            {
                PropertyName = nameof(dto.Room),
                PropertyValue = dto.Room
            });

        if (!dto.Floor.Equals(sourceDto.Floor))
            patchList.Add(new PatchModel
            {
                PropertyName = nameof(dto.Floor),
                PropertyValue = dto.Floor
            });

        if (!dto.DeskNumber.Equals(sourceDto.DeskNumber))
            patchList.Add(new PatchModel
            {
                PropertyName = nameof(dto.DeskNumber),
                PropertyValue = dto.DeskNumber
            });

        await _unitOfWork.Workplaces.PatchAsync(id, patchList);
        return await _unitOfWork.Commit();
    }
}