using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.DomainEventHandlers;

/// <summary>
/// A domain event handler that marks a scooter as rented whenever a customer requests a rent.
/// </summary>
public class RentScooterWhenRentIsRequested : DomainEventHandlerBase<RentRequestedEvent>
{
    private readonly IScooterRepository _scooterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RentScooterWhenRentIsRequested"/> class.
    /// </summary>
    /// <param name="scooterRepository">The customer repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RentScooterWhenRentIsRequested(
        IScooterRepository scooterRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _scooterRepository = scooterRepository;
    }

    /// <inheritdoc/>
    protected override async Task<Response<Nothing>> Handle(RentRequestedEvent ev)
    {
        return await _scooterRepository.GetById(ev.Rent.ScooterId)
            .ThenRequire(scooter => scooter.Rent(ev.Rent.Id))
            .ThenIfSuccess(_scooterRepository.Save)
            .ThenToResponse();
    }
}
