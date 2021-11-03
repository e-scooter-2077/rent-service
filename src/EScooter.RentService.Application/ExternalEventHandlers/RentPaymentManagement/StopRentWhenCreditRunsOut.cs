using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.RentPaymentManagement
{
    /// <summary>
    /// An external event published by the rent payment context when a customer's credit runs out during a rent.
    /// </summary>
    /// <param name="RentId">The Id of the rent.</param>
    public record CreditExhaustedForRent(Guid RentId) : ExternalEvent;

    /// <summary>
    /// An external event handler that stops a rent when a customer's credit runs out during a rent.
    /// </summary>
    public class StopRentWhenCreditRunsOut : ExternalEventHandlerBase<CreditExhaustedForRent>
    {
        private readonly IRentRepository _rentRepository;
        private readonly ITimestampProvider _timestampProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopRentWhenCreditRunsOut"/> class.
        /// </summary>
        /// <param name="rentRepository">The rent repository.</param>
        /// <param name="timestampProvider">The timestamp provider.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public StopRentWhenCreditRunsOut(
            IRentRepository rentRepository,
            ITimestampProvider timestampProvider,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _rentRepository = rentRepository;
            _timestampProvider = timestampProvider;
        }

        /// <inheritdoc/>
        protected override async Task<Response<Nothing>> Handle(CreditExhaustedForRent ev)
        {
            return await _rentRepository.GetById(ev.RentId)
                .ThenRequire(rent => rent.Stop(new(RentStopReason.CreditExhausted, _timestampProvider.Now)))
                .ThenIfSuccess(_rentRepository.Save)
                .ThenToResponse();
        }
    }
}
