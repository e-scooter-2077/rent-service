using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.RentPaymentManagement;

/// <summary>
/// An external event published by the rent payment context whenever the initial payment
/// for a rent is authorized.
/// </summary>
/// <param name="RentId">The Id of the rent.</param>
public record RentPaymentAuthorized(Guid RentId, Timestamp StartTime) : ExternalEvent;

/// <summary>
/// An external event handler that confirms a rent after the payment for that rent has been authorized.
/// </summary>
public class ConfirmRentWhenPaymentIsAuthorized : ExternalEventHandlerBase<RentPaymentAuthorized>
{
    private readonly IRentRepository _rentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmRentWhenPaymentIsAuthorized"/> class.
    /// </summary>
    /// <param name="rentRepository">The rent repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public ConfirmRentWhenPaymentIsAuthorized(
        IRentRepository rentRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _rentRepository = rentRepository;
    }

    /// <inheritdoc/>
    protected override async Task<Response<Nothing>> Handle(RentPaymentAuthorized ev)
    {
        return await _rentRepository.GetById(ev.RentId)
            .ThenRequire(rent => rent.Confirm(new(ev.StartTime)))
            .ThenIfSuccess(_rentRepository.Save)
            .ThenToResponse();
    }
}
