using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents;

public record ScooterDisabled(Guid ScooterId) : ExternalEvent;

public class PropagateScooterDisabledEvent : DomainEventPropagator<ScooterDisabledEvent>
{
    public PropagateScooterDisabledEvent(IExternalEventPublisher publisher) : base(publisher)
    {
    }

    protected override ExternalEvent ConvertToExternalEvent(ScooterDisabledEvent ev) =>
        new ScooterDisabled(ev.Scooter.Id);
}
