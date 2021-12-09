using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace EScooter.RentService.Domain.Aggregates.RentAggregate
{
    /// <summary>
    /// Contains information about the end of a rent.
    /// </summary>
    /// <param name="Reason">The reason for the end of the rent.</param>
    /// <param name="Timestamp">The instant in which the end of the rent occurred.</param>
    public record RentStopInfo(RentStopReason Reason, Timestamp Timestamp);

    /// <summary>
    /// Describes one of the reasons for which a rent can end.
    /// </summary>
    public enum RentStopReason
    {
        /// <summary>
        /// Indicates that a rent ended by will of the customer.
        /// </summary>
        StoppedByCustomer,

        /// <summary>
        /// Indicates that a rent ended due to the scooter leaving its current area of service.
        /// </summary>
        OutOfArea,

        /// <summary>
        /// Indicates that a rent ended due to the customer running out of credit during the ride.
        /// </summary>
        CreditExhausted,

        /// <summary>
        /// Indicates that a rent ended due to the battery of the scooter falling below the standby threshold.
        /// </summary>
        Standby,

        /// <summary>
        /// Indicates that a rent ended due to the scooter being disabled by an administrator.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that a rent ended due to the scooter becoming not rentable.
        /// </summary>
        NotRentable
    }
}
