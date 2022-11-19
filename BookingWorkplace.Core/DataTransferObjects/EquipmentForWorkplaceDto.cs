﻿namespace BookingWorkplace.Core.DataTransferObjects;

public class EquipmentForWorkplaceDto
{
    public Guid Id { get; set; }

    public Guid EquipmentId { get; set; }
    public EquipmentDto Equipment { get; set; }

    public Guid WorkplacesId { get; set; }
    public WorkplaceDto Workplace { get; set; }

    public int Count {get; set; }
}