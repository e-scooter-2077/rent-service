using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.DomainEventHandlers;

public class EndRentForCustomerWhenRentEnds : DomainEventHandlerBase<RentEndedEvent>
{
    private readonly ICustomerRepository _customerRepository;

    public EndRentForCustomerWhenRentEnds(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<Response<Nothing>> Handle(RentEndedEvent ev)
    {
        return await _customerRepository.GetById(ev.Rent.CustomerId)
            .ThenIfSuccess(customer => customer.EndRent())
            .ThenIfSuccess(_customerRepository.Save)
            .ThenToResponse();
    }
}
