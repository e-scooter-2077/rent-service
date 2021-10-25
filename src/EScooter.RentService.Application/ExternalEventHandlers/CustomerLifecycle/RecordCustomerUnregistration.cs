using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.CustomerLifecycle
{
    /// <summary>
    /// An external event published by the customer context whenever a customer is unregistered from the system.
    /// </summary>
    /// <param name="Id">The Id of the customer.</param>
    public record CustomerDeleted(Guid Id) : ExternalEvent;

    /// <summary>
    /// An external event handler that records the unregistration of a customer in the Customer context, removing
    /// the corresponding <see cref="Customer"/> inside the rent context.
    /// </summary>
    public class RecordCustomerUnregistration : ExternalEventHandlerBase<CustomerDeleted>
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordCustomerUnregistration"/> class.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public RecordCustomerUnregistration(ICustomerRepository customerRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
        }

        /// <inheritdoc/>
        protected override async Task<Response<Nothing>> Handle(CustomerDeleted ev)
        {
            return await _customerRepository.GetById(ev.Id)
                .ThenIfSuccess(_customerRepository.Remove)
                .ThenToResponse();
        }
    }
}
