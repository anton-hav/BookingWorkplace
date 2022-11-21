﻿using AutoMapper;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    /// Executes a retrieval query with the corresponding "id" of the entity in the data source.
    /// If a matching entity is found, a mapping is performed from entity type to data transfer object type.
    /// The result of the mapping is returned
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns>data transfer object corresponding to the id</returns>
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
    /// Execute an entity search on the data source by IQueryStringParameters.SearchString. Execute a sort,
    /// and skips the number equal to the product of IQueryStringParameters.CurrentPage and IQueryStringParameters.PageSize.
    /// Retrieves *IQueryStringParameters.PageSize* of the following records. Execute mapping.
    /// </summary>
    /// <param name="parameters">object that implements IQueryStringParameters</param>
    /// <returns>PagedList of data transfer objects</returns>
    public PagedList<EquipmentDto> GetEquipmentByQueryStringParameters(IQueryStringParameters parameters)
    {
        var query = _unitOfWork.Equipment
            .Get()
            .AsNoTracking();

        if (!String.IsNullOrEmpty(parameters.SearchString))
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
    /// Checks for existing a record in the data source that matches the parameters.
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
    /// Execute mapping from data transfer object to entity type and create a new record in the data source.
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
    /// Executes record patching in the data source
    /// </summary>
    /// <param name="id">unique identifier of record</param>
    /// <param name="dto">modified data transfer object</param>
    /// <returns>the number of records successfully changed</returns>
    public async Task<int> UpdateAsync(Guid id, EquipmentDto dto)
    {
        var sourceDto = await GetEquipmentByIdAsync(id);

        var patchList = new List<PatchModel>();

        if (!dto.Type.Equals(sourceDto.Type))
        {
            patchList.Add(new PatchModel()
            {
                PropertyName = nameof(dto.Type),
                PropertyValue = dto.Type
            });
        }

        await _unitOfWork.Equipment.PatchAsync(id, patchList);
        return await _unitOfWork.Commit();
    }


    public async Task<int> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}