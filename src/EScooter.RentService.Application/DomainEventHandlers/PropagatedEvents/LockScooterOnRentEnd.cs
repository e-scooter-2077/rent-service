using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents
{
    public record LockScooter(Guid ScooterId) : ExternalEvent;

    public class LockScooterOnRentEnd : DomainEventPropagator<RentEndedEvent>
    {
        public LockScooterOnRentEnd(IExternalEventPublisher publisher) : base(publisher)
        {
        }

        protected override ExternalEvent ConvertToExternalEvent(RentEndedEvent ev) =>
            new LockScooter(ev.Rent.ScooterId);
    }
}
