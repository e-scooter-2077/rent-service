using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.Commands.Scooters
{
    /// <summary>
    /// An application command to enable a scooter.
    /// </summary>
    public class EnableScooter
    {
        /// <summary>
        /// The command data for the <see cref="EScooter.RentService.Application.Commands.Scooters.EnableScooter"/> command.
        /// </summary>
        /// <param name="ScooterId">The Id of the scooter to enable.</param>
        public record Command(Guid ScooterId) : CommandBase<Nothing>;

        /// <summary>
        /// The handler for the <see cref="EnableScooter"/> command.
        /// </summary>
        public class Handler : UnitOfWorkHandler<Command, Nothing>
        {
            private readonly IScooterRepository _scooterRepository;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="scooterRepository">The scooter repository.</param>
            /// <param name="unitOfWork">The unit of work.</param>
            public Handler(IScooterRepository scooterRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
            {
                _scooterRepository = scooterRepository;
            }

            /// <inheritdoc/>
            protected override async Task<Response<Nothing>> HandleRequest(Command request)
            {
                return await _scooterRepository.GetById(request.ScooterId)
                    .ThenIfSuccess(scooter => scooter.Enable())
                    .ThenIfSuccess(_scooterRepository.Save)
                    .ThenToResponse();
            }
        }
    }
}
