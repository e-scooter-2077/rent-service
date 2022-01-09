using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;

namespace EScooter.RentService.Application.DomainEventHandlers.PropagatedEvents;

public record ScooterEnabled(Guid ScooterId) : ExternalEvent;

public class PropagateScooterEnabledEvent : DomainEventPropagator<ScooterEnabledEvent>
{
    public PropagateScooterEnabledEvent(IExternalEventPublisher publisher) : base(publisher)
    {
    }

    protected override ExternalEvent ConvertToExternalEvent(ScooterEnabledEvent ev) =>
        new ScooterEnabled(ev.Scooter.Id);
}
