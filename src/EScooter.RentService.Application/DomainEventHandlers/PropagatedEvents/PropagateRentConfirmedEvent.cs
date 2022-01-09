using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents;

/// <summary>
/// An external event published when a rent is confirmed.
/// </summary>
/// <param name="RentId">The Id of the confirmed rent.</param>
/// <param name="CustomerId">The Id of the customer.</param>
/// <param name="ScooterId">The Id of the rented scooter.</param>
/// <param name="Timestamp">The confirmation timestamp.</param>
public record RentConfirmed(
    Guid RentId,
    Guid CustomerId,
    Guid ScooterId,
    Timestamp Timestamp) : ExternalEvent;

/// <summary>
/// An event propagator for the <see cref="RentConfirmedEvent"/>.
/// </summary>
public class PropagateRentConfirmedEvent : DomainEventPropagator<RentConfirmedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropagateRentConfirmedEvent"/> class.
    /// </summary>
    /// <param name="publisher">The event publisher.</param>
    public PropagateRentConfirmedEvent(IExternalEventPublisher publisher) : base(publisher)
    {
    }

    /// <inheritdoc/>
    protected override ExternalEvent ConvertToExternalEvent(RentConfirmedEvent ev) =>
        new RentConfirmed(
            ev.Rent.Id,
            ev.Rent.CustomerId,
            ev.Rent.ScooterId,
            ev.ConfirmationInfo.Timestamp);
}
