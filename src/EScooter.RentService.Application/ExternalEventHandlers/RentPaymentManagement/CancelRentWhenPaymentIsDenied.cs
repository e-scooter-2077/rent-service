using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.RentPaymentManagement;

/// <summary>
/// An external event published by the rent payment context whenever the initial payment for a rent
/// cannot be performed.
/// </summary>
/// <param name="RentId">The Id of the rent.</param>
public record RentPaymentDenied(Guid RentId) : ExternalEvent;

/// <summary>
/// An external event handler that cancels a rent when payment for that rent is denied.
/// </summary>
public class CancelRentWhenPaymentIsDenied : ExternalEventHandlerBase<RentPaymentDenied>
{
    private readonly IRentRepository _rentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelRentWhenPaymentIsDenied"/> class.
    /// </summary>
    /// <param name="rentRepository">The rent repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CancelRentWhenPaymentIsDenied(
        IRentRepository rentRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _rentRepository = rentRepository;
    }

    /// <inheritdoc/>
    protected override async Task<Response<Nothing>> Handle(RentPaymentDenied ev)
    {
        return await _rentRepository.GetById(ev.RentId)
            .ThenRequire(rent => rent.Cancel(new(RentCancellationReason.CreditInsufficient)))
            .ThenIfSuccess(_rentRepository.Save)
            .ThenToResponse();
    }
}
