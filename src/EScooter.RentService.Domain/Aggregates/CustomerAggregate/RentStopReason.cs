namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
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
        BatteryLow
    }
}
