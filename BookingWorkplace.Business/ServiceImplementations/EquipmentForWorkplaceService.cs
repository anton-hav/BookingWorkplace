﻿using AutoMapper;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.DataBase.Entities;

namespace BookingWorkplace.Business.ServiceImplementations;

public class EquipmentForWorkplaceService : IEquipmentForWorkplaceService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentForWorkplaceService(IMapper mapper, 
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets the object that corresponding Id  from the data source and returns as a response.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.EquipmentForWorkplaces.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentException("No record of the equipment for workplace was found in the database.", nameof(id));

        var dto = _mapper.Map<EquipmentForWorkplaceDto>(entity);
        return dto;
    }

    public PagedList<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByQueryStringParameters(IQueryStringParameters parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<List<EquipmentForWorkplaceDto>> GetAvailableEquipmentForWorkplaceByWorkplaceId(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsEquipmentForWorkplaceExistAsync(string typeName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new record in the data source
    /// </summary>
    /// <param name="dto">data transfer object</param>
    /// <returns>the number of successfully created records in the data source</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<int> CreateEquipmentForWorkplaceAsync(EquipmentForWorkplaceDto dto)
    {
        var entity = _mapper.Map<EquipmentForWorkplace>(dto);

        if (entity == null)
            throw new ArgumentException("Mapping EquipmentForWorkplaceDto to EquipmentForWorkplace was not possible.", nameof(dto));

        await _unitOfWork.EquipmentForWorkplaces.AddAsync(entity);
        var result = await _unitOfWork.Commit();
        return result;
    }

    public async Task<int> UpdateAsync(Guid id, EquipmentForWorkplaceDto equipment)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Remove a record from the data source
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
        else
        {
            throw new ArgumentException("The equipment for workplace for removing doesn't exist", nameof(id));
        }
    }
}