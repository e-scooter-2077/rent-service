namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Describes one of the reasons for which a rent can be cancelled.
    /// </summary>
    public enum RentCancellationReason
    {
        /// <summary>
        /// Indicates that a rent has been cancelled due to the customer not having enough credit in his balance.
        /// </summary>
        CreditInsufficient,

        /// <summary>
        /// Indicates that a rent has been cancelled due to the unavailability of the requested scooter.
        /// This can be caused by simultaneous rents by different users or by the impossibility to physically unlock
        /// the scooter.
        /// </summary>
        ScooterUnavailable,

        /// <summary>
        /// Indicates that a rent has been cancelled due to a generic error.
        /// </summary>
        InternalError
    }
}
