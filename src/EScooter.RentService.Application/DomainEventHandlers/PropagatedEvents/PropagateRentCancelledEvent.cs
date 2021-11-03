using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents
{
    /// <summary>
    /// An external event published when a rent is cancelled.
    /// </summary>
    /// <param name="RentId">The Id of the cancelled rent.</param>
    /// <param name="CustomerId">The Id of the customer.</param>
    /// <param name="ScooterId">The Id of the rented scooter.</param>
    /// <param name="Reason">The reason for which the cancellation happened.</param>
    public record RentCancelled(
        Guid RentId,
        Guid CustomerId,
        Guid ScooterId,
        string Reason) : ExternalEvent;

    /// <summary>
    /// An event propagator for the <see cref="RentCancelledEvent"/>.
    /// </summary>
    public class PropagateRentCancelledEvent : DomainEventPropagator<RentCancelledEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropagateRentCancelledEvent"/> class.
        /// </summary>
        /// <param name="publisher">The event publisher.</param>
        public PropagateRentCancelledEvent(IExternalEventPublisher publisher) : base(publisher)
        {
        }

        /// <inheritdoc/>
        protected override ExternalEvent ConvertToExternalEvent(RentCancelledEvent ev) =>
            new RentCancelled(
                ev.Rent.Id,
                ev.Rent.CustomerId,
                ev.Rent.ScooterId,
                ev.CancellationInfo.Reason.ToString());
    }
}
