using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace EScooter.RentService.Domain.Aggregates.RentAggregate;

/// <summary>
/// Contains information about the confirmation of a rent.
/// </summary>
/// <param name="Timestamp">The instant in which the confirmation occurred.</param>
public record RentConfirmationInfo(Timestamp Timestamp);
