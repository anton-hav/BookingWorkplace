namespace BookingWorkplace.Core;

/// <summary>
/// The class provides information about equipment movement from one workplace to a new one.
/// </summary>
public class EquipmentMovementData
{
    /// <summary>
    /// A unique identifier of equipment for workplace
    /// </summary>
    public Guid EquipmentId { get; set; }
    public Guid PreviousWorkplace { get; set; }
    public Guid DestinationWorkplace { get; set; }
    public DateTime DateOfMovement { get; set; }
}