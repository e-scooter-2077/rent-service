using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents
{
    /// <summary>
    /// An external event published when a rent is requested.
    /// </summary>
    /// <param name="RentId">The Id of the requested rent.</param>
    /// <param name="CustomerId">The Id of the customer.</param>
    /// <param name="ScooterId">The Id of the rented scooter.</param>
    public record RentRequested(
        Guid RentId,
        Guid CustomerId,
        Guid ScooterId) : ExternalEvent;

    /// <summary>
    /// An event propagator for the <see cref="RentRequestedEvent"/>.
    /// </summary>
    public class PropagateRentRequestedEvent : DomainEventPropagator<RentRequestedEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropagateRentRequestedEvent"/> class.
        /// </summary>
        /// <param name="publisher">The event publisher.</param>
        public PropagateRentRequestedEvent(IExternalEventPublisher publisher) : base(publisher)
        {
        }

        /// <inheritdoc/>
        protected override ExternalEvent ConvertToExternalEvent(RentRequestedEvent ev) =>
            new RentRequested(
                ev.Rent.Id,
                ev.Customer.Id,
                ev.Rent.ScooterId);
    }
}
