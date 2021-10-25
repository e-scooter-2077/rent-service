using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.Commands.Rents
{
    /// <summary>
    /// An application command to stop the ongoing rent for a customer.
    /// </summary>
    public static class StopRent
    {
        /// <summary>
        /// The command data for the <see cref="EScooter.RentService.Application.Commands.Rents.StopRent"/> command.
        /// </summary>
        /// <param name="CustomerId">The Id of the customer stopping the rent.</param>
        public record Command(Guid CustomerId) : CommandBase<Nothing>;

        /// <summary>
        /// The handler for the <see cref="StopRent"/> command.
        /// </summary>
        public class Handler : UnitOfWorkHandler<Command, Nothing>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly ITimestampProvider _timestampProvider;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="customerRepository">The customer repository.</param>
            /// <param name="timestampProvider">The timestamp provider.</param>
            /// <param name="unitOfWork">The unit of work.</param>
            public Handler(
                ICustomerRepository customerRepository,
                ITimestampProvider timestampProvider,
                IUnitOfWork unitOfWork) : base(unitOfWork)
            {
                _customerRepository = customerRepository;
                _timestampProvider = timestampProvider;
            }

            /// <inheritdoc/>
            protected override async Task<Response<Nothing>> HandleRequest(Command request)
            {
                return await _customerRepository.GetById(request.CustomerId)
                    .ThenRequire(customer => customer.StopOngoingRent(new(RentStopReason.StoppedByCustomer, _timestampProvider.Now)))
                    .ThenIfSuccess(_customerRepository.Save)
                    .ThenToResponse();
            }
        }
    }
}
