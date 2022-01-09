using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.Commands.Rents;

/// <summary>
/// An application command to stop the ongoing rent for a customer.
/// </summary>
public static class StopRent
{
    /// <summary>
    /// The command data for the <see cref="EScooter.RentService.Application.Commands.Rents.StopRent"/> command.
    /// </summary>
    /// <param name="RentId">The Id of the rent to be stopped.</param>
    public record Command(Guid RentId) : CommandBase<Nothing>;

    /// <summary>
    /// The handler for the <see cref="StopRent"/> command.
    /// </summary>
    public class Handler : UnitOfWorkHandler<Command, Nothing>
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
        protected override async Task<Response<Nothing>> HandleRequest(Command request)
        {
            return await _rentRepository.GetById(request.RentId)
                .ThenRequire(rent => rent.Stop(new(RentStopReason.StoppedByCustomer, _timestampProvider.Now)))
                .ThenIfSuccess(_rentRepository.Save)
                .ThenToResponse();
        }
    }
}
