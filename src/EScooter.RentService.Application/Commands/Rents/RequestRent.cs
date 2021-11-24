using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Time;
using EScooter.RentService.Application.Queries;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.Commands.Rents
{
    /// <summary>
    /// An application command to request a rent for a customer.
    /// </summary>
    public static class RequestRent
    {
        /// <summary>
        /// The command data for the <see cref="EScooter.RentService.Application.Commands.Rents.RequestRent"/> command.
        /// </summary>
        /// <param name="CustomerId">The Id of the customer performing the rent.</param>
        /// <param name="ScooterId">The Id of the scooter to be rented.</param>
        public record Command(Guid CustomerId, Guid ScooterId) : CommandBase<RentSnapshot>;

        /// <summary>
        /// The handler for the <see cref="RequestRent"/> command.
        /// </summary>
        public class Handler : UnitOfWorkHandler<Command, RentSnapshot>
        {
            private readonly IRentRepository _rentRepository;
            private readonly ITimestampProvider _timestampProvider;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="rentRepository">The rent repository.</param>
            /// <param name="timestampProvider">The timestamp provider.</param>
            /// <param name="unitOfWork">The unit of work.</param>
            public Handler(
                IRentRepository rentRepository,
                ITimestampProvider timestampProvider,
                IUnitOfWork unitOfWork) : base(unitOfWork)
            {
                _rentRepository = rentRepository;
                _timestampProvider = timestampProvider;
            }

            /// <inheritdoc/>
            protected override Task<Response<RentSnapshot>> HandleRequest(Command request)
            {
                var rent = Rent.Create(request.ScooterId, request.CustomerId, _timestampProvider.Now);
                _rentRepository.Save(rent);
                return Task.FromResult<Response<RentSnapshot>>(new RentSnapshot(
                    rent.Id,
                    rent.CustomerId,
                    rent.ScooterId,
                    rent.RequestTimestamp,
                    rent.ConfirmationInfo,
                    rent.CancellationInfo,
                    rent.StopInfo));
            }
        }
    }
}
