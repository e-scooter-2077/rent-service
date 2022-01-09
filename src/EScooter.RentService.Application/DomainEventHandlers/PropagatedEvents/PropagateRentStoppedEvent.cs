using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents;

/// <summary>
/// An external event published when a rent is stopped.
/// </summary>
/// <param name="RentId">The Id of the stopped rent.</param>
/// <param name="CustomerId">The Id of the customer.</param>
/// <param name="ScooterId">The Id of the rented scooter.</param>
/// <param name="Timestamp">The stop timestamp.</param>
/// <param name="Reason">The reason for which the stop happened.</param>
public record RentStopped(
    Guid RentId,
    Guid CustomerId,
    Guid ScooterId,
    Timestamp Timestamp,
    string Reason) : ExternalEvent;

/// <summary>
/// An event propagator for the <see cref="RentStoppedEvent"/>.
/// </summary>
public class PropagateRentStoppedEvent : DomainEventPropagator<RentStoppedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropagateRentStoppedEvent"/> class.
    /// </summary>
    /// <param name="publisher">The event propagator.</param>
    public PropagateRentStoppedEvent(IExternalEventPublisher publisher) : base(publisher)
    {
    }

    /// <inheritdoc/>
    protected override ExternalEvent ConvertToExternalEvent(RentStoppedEvent ev) =>
        new RentStopped(
            ev.Rent.Id,
            ev.Rent.CustomerId,
            ev.Rent.ScooterId,
            ev.StopInfo.Timestamp,
            ev.StopInfo.Reason.ToString());
}
