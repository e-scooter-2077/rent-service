using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents;

public record UnlockScooter(Guid ScooterId) : ExternalEvent;

public class UnlockScooterOnRentConfirmation : DomainEventPropagator<RentConfirmedEvent>
{
    public UnlockScooterOnRentConfirmation(IExternalEventPublisher publisher) : base(publisher)
    {
    }

    protected override ExternalEvent ConvertToExternalEvent(RentConfirmedEvent ev) =>
        new UnlockScooter(ev.Rent.ScooterId);
}
