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

namespace EScooter.RentService.Application.ExternalEventHandlers.RentPaymentManagement
{
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
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRentWhenPaymentIsDenied"/> class.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public CancelRentWhenPaymentIsDenied(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
        }

        /// <inheritdoc/>
        protected override async Task<Response<Nothing>> Handle(RentPaymentDenied ev)
        {
            return await _customerRepository.GetByRent(ev.RentId)
                .ThenRequire(customer => customer.CancelOngoingRent(new(RentCancellationReason.CreditInsufficient)))
                .ThenIfSuccess(_customerRepository.Save)
                .ThenToResponse();
        }
    }
}
