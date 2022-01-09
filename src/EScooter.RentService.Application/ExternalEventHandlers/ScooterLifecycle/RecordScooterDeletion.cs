using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.ScooterLifecycle;

/// <summary>
/// An external event published by the scooter data context whenever a new scooter is removed from the system.
/// </summary>
/// <param name="Id">The Id of the scooter.</param>
public record ScooterDeleted(Guid Id) : ExternalEvent;

/// <summary>
/// An external event handler that records the deletion of a scooter in the scooter data context, deleting
/// the corresponding <see cref="Scooter"/> inside the rent context.
/// </summary>
public class RecordScooterDeletion : ExternalEventHandlerBase<ScooterDeleted>
{
    private readonly IScooterRepository _scooterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordScooterDeletion"/> class.
    /// </summary>
    /// <param name="scooterRepository">The scooter repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RecordScooterDeletion(IScooterRepository scooterRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _scooterRepository = scooterRepository;
    }

    /// <inheritdoc/>
    protected override async Task<Response<Nothing>> Handle(ScooterDeleted ev)
    {
        return await _scooterRepository.GetById(ev.Id)
            .ThenIfSuccess(_scooterRepository.Remove)
            .ThenToResponse();
    }
}
