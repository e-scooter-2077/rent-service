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
/// A domain event handler that clears the rent on a scooter whenever a rent ends.
/// </summary>
public class ClearRentOnScooterWhenRentEnds : DomainEventHandlerBase<RentEndedEvent>
{
    private readonly IScooterRepository _scooterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearRentOnScooterWhenRentEnds"/> class.
    /// </summary>
    /// <param name="scooterRepository">The scooter repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public ClearRentOnScooterWhenRentEnds(
        IScooterRepository scooterRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _scooterRepository = scooterRepository;
    }

    /// <inheritdoc/>
    protected override async Task<Response<Nothing>> Handle(RentEndedEvent ev)
    {
        return await _scooterRepository.GetById(ev.Rent.ScooterId)
            .ThenIfSuccess(scooter => scooter.ClearRent())
            .ThenIfSuccess(_scooterRepository.Save)
            .ThenToResponse();
    }
}
