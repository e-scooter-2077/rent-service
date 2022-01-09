using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;
using System.Threading.Tasks;
using static EasyDesk.CleanArchitecture.Application.Responses.ResponseImports;

namespace EScooter.RentService.Application.ExternalEventHandlers.CustomerLifecycle;

/// <summary>
/// An external event published by the customer context whenever a new customer is registered to the system.
/// </summary>
/// <param name="Id">The Id of the customer.</param>
public record CustomerCreated(Guid Id) : ExternalEvent;

/// <summary>
/// An external event handler that records the registration of a customer in the Customer context, creating
/// a <see cref="Customer"/> inside the rent context.
/// </summary>
public class RecordCustomerRegistration : ExternalEventHandlerBase<CustomerCreated>
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordCustomerRegistration"/> class.
    /// </summary>
    /// <param name="customerRepository">The customer repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RecordCustomerRegistration(ICustomerRepository customerRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc/>
    protected override Task<Response<Nothing>> Handle(CustomerCreated ev)
    {
        var customer = Customer.Create(ev.Id);
        _customerRepository.Save(customer);
        return OkAsync;
    }
}
