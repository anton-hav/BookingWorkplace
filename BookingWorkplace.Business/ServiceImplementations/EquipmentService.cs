using AutoMapper;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingWorkplace.Business.ServiceImplementations;

public class EquipmentService : IEquipmentService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///     Executes a retrieval query with the corresponding "id" of the entity in the data source.
    ///     If a matching entity is found, a mapping is performed from entity type to data transfer object type.
    ///     The result of the mapping is returned
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns><see cref="EquipmentDto" /> corresponding to the id</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentDto> GetEquipmentByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Equipment.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentException("No record of the equipment was found in the database.", nameof(id));

        var dto = _mapper.Map<EquipmentDto>(entity);
        return dto;
    }

    /// <summary>
    ///     Gets equipment by Id includes equipment for workplaces info and workplace info
    /// </summary>
    /// <param name="id">unique identifier as a <see cref="Guid" /></param>
    /// <returns><see cref="EquipmentDto" /> corresponding to the id</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentDto> GetEquipmentWithFullInfoByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Equipment
            .Get()
            .Where(entity => entity.Id.Equals(id))
            .Include(entity => entity.EquipmentForWorkplaces)
            .ThenInclude(eFW => eFW.Workplace)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new ArgumentException("No record of the equipment was found in the database.", nameof(id));

        var dto = _mapper.Map<EquipmentDto>(entity);
        return dto;
    }

    /// <summary>
    ///     Gets all equipment records from the data source.
    /// </summary>
    /// <returns>The <see cref="List&lt;T&gt;" /> of <see cref="EquipmentDto" /></returns>
    public async Task<List<EquipmentDto>> GetAllEquipmentAsync()
    {
        // It is still not a bad idea for the entity of the type of equipment
        var entities = await _unitOfWork.Equipment
            .Get()
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<EquipmentDto>>(entities);
    }


    /// <summary>
    ///     Execute an entity search on the data source by IQueryStringParameters.SearchString. Execute a sort,
    ///     and skips the number equal to the product of IQueryStringParameters.CurrentPage and
    ///     IQueryStringParameters.PageSize.
    ///     Retrieves *IQueryStringParameters.PageSize* of the following records. Execute mapping.
    /// </summary>
    /// <param name="parameters">object that implements IQueryStringParameters</param>
    /// <returns><see cref="PagedList&lt;T&gt;" /> of <see cref="EquipmentDto" /></returns>
    public PagedList<EquipmentDto> GetEquipmentByQueryStringParameters(IQueryStringParameters parameters)
    {
        var query = _unitOfWork.Equipment
            .Get()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(parameters.SearchString))
            query = query.Where(entity => entity.Type.Contains(parameters.SearchString));

        var mappedQuery = query
            .OrderBy(entity => entity.Type)
            .Select(entity => _mapper.Map<EquipmentDto>(entity));

        if (mappedQuery == null)
            throw new ArgumentException("Failed to find records in the database that match the specified parameters. ",
                nameof(parameters));

        var list = PagedList<EquipmentDto>.ToPagedList(mappedQuery, parameters);

        return list;
    }

    /// <summary>
    ///     Gets available equipment for adding to the current workplace.
    /// </summary>
    /// <param name="id">a workplace unique identifier</param>
    /// <returns><see cref="List{T}" /> where T is <see cref="EquipmentDto" /></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<List<EquipmentDto>> GetAvailableEquipmentToAddToWorkplaceByWorkplaceIdAsync(Guid id)
    {
        var list = await _unitOfWork.Equipment
            .Get()
            .Where(equip => !equip.EquipmentForWorkplaces.Any(efw => efw.WorkplaceId.Equals(id)))
            .AsNoTracking()
            .Select(equip => _mapper.Map<EquipmentDto>(equip))
            .ToListAsync();

        if (list == null)
            throw new ArgumentException(nameof(id));

        return list;
    }

    /// <summary>
    ///     Checks for existing a record in the data source that matches the parameters.
    /// </summary>
    /// <param name="typeName">equipment type name</param>
    /// <returns>A boolean (true if the record exists, or false if it does not exist)</returns>
    public async Task<bool> IsEquipmentExistAsync(string typeName)
    {
        var entity = await _unitOfWork.Equipment
            .Get()
            .FirstOrDefaultAsync(entity => entity.Type.Equals(typeName));

        return entity != null;
    }

    /// <summary>
    ///     Execute mapping from data transfer object to entity type and create a new record in the data source.
    /// </summary>
    /// <param name="dto">data transfer object</param>
    /// <returns>the number of successfully created records in the data source</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> CreateEquipmentAsync(EquipmentDto dto)
    {
        var entity = _mapper.Map<Equipment>(dto);

        if (entity == null)
            throw new ArgumentException("Mapping EquipmentDto to Equipment was not possible.", nameof(dto));

        await _unitOfWork.Equipment.AddAsync(entity);
        var result = await _unitOfWork.Commit();
        return result;
    }

    /// <summary>
    ///     Executes record patching in the data source
    /// </summary>
    /// <param name="id">unique identifier of record</param>
    /// <param name="dto">modified data transfer object</param>
    /// <returns>the number of records successfully changed</returns>
    public async Task<int> UpdateAsync(Guid id, EquipmentDto dto)
    {
        var sourceDto = await GetEquipmentByIdAsync(id);

        var patchList = new List<PatchModel>();

        if (!dto.Type.Equals(sourceDto.Type))
            patchList.Add(new PatchModel
            {
                PropertyName = nameof(dto.Type),
                PropertyValue = dto.Type
            });

        await _unitOfWork.Equipment.PatchAsync(id, patchList);
        return await _unitOfWork.Commit();
    }

    /// <summary>
    ///     Remove a record from the data source
    /// </summary>
    /// <param name="id">unique identifier of record</param>
    /// <returns>the number of records successfully removed</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.Equipment.GetByIdAsync(id);

        if (entity != null)
        {
            _unitOfWork.Equipment.Remove(entity);
            return await _unitOfWork.Commit();
        }

        throw new ArgumentException("The equipment for removing doesn't exist", nameof(id));
    }
}