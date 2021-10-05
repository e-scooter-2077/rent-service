namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Contains information about how a rent was cancelled.
    /// </summary>
    /// <param name="Reason">The reason for which the rent was cancelled.</param>
    public record RentCancellationInfo(RentCancellationReason Reason);
}
