using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;
using System.Threading.Tasks;
using static EasyDesk.CleanArchitecture.Application.Responses.ResponseImports;

namespace EScooter.RentService.Application.ExternalEventHandlers.ScooterLifecycle;

/// <summary>
/// An external event published by the scooter data context whenever a new scooter is added to the system.
/// </summary>
/// <param name="Id">The Id of the scooter.</param>
public record ScooterCreated(Guid Id) : ExternalEvent;

/// <summary>
/// An external event handler that records the creation of a scooter in the scooter data context, creating
/// a <see cref="Scooter"/> inside the rent context.
/// </summary>
public class RecordScooterCreation : ExternalEventHandlerBase<ScooterCreated>
{
    private readonly IScooterRepository _scooterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordScooterCreation"/> class.
    /// </summary>
    /// <param name="scooterRepository">The scooter repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RecordScooterCreation(IScooterRepository scooterRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _scooterRepository = scooterRepository;
    }

    /// <inheritdoc/>
    protected override Task<Response<Nothing>> Handle(ScooterCreated ev)
    {
        var scooter = Scooter.Create(ev.Id);
        _scooterRepository.Save(scooter);
        return OkAsync;
    }
}
