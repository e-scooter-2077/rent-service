using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Contains information about the end of a rent.
    /// </summary>
    /// <param name="Reason">The reason for the end of the rent.</param>
    /// <param name="Timestamp">The instant in which the end of the rent occurred.</param>
    public record RentStopInfo(RentStopReason Reason, Timestamp Timestamp);
}
