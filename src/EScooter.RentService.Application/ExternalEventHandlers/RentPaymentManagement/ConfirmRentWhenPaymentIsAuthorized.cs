﻿using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.RentPaymentManagement
{
    /// <summary>
    /// An external event published by the rent payment context whenever the initial payment
    /// for a rent is authorized.
    /// </summary>
    /// <param name="RentId">The Id of the rent.</param>
    public record RentPaymentAuthorized(Guid RentId) : ExternalEvent;

    /// <summary>
    /// An external event handler that confirms a rent after the payment for that rent has been authorized.
    /// </summary>
    public class ConfirmRentWhenPaymentIsAuthorized : ExternalEventHandlerBase<RentPaymentAuthorized>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITimestampProvider _timestampProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmRentWhenPaymentIsAuthorized"/> class.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="timestampProvider">The timestamp provider.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public ConfirmRentWhenPaymentIsAuthorized(
            ICustomerRepository customerRepository,
            ITimestampProvider timestampProvider,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _customerRepository = customerRepository;
            _timestampProvider = timestampProvider;
        }

        /// <inheritdoc/>
        protected override async Task<Response<Nothing>> Handle(RentPaymentAuthorized ev)
        {
            return await _customerRepository.GetByRent(ev.RentId)
                .ThenRequire(customer => customer.ConfirmOngoingRent(new(_timestampProvider.Now)))
                .ThenIfSuccess(_customerRepository.Save)
                .ThenToResponse();
        }
    }
}