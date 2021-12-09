using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Time;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System.Threading.Tasks;
using static EasyDesk.CleanArchitecture.Application.Responses.ResponseImports;

namespace EScooter.RentService.Application.DomainEventHandlers
{
    public class StopRentWhenScooterBecomesNotRentable : DomainEventHandlerBase<ScooterBecameNotRentableEvent>
    {
        private readonly IRentRepository _rentRepository;
        private readonly ITimestampProvider _timestampProvider;

        public StopRentWhenScooterBecomesNotRentable(
            IRentRepository rentRepository,
            ITimestampProvider timestampProvider,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _rentRepository = rentRepository;
            _timestampProvider = timestampProvider;
        }

        protected override async Task<Response<Nothing>> Handle(ScooterBecameNotRentableEvent ev)
        {
            if (ev.Scooter.OngoingRentId.IsAbsent)
            {
                return Ok;
            }
            var rentId = ev.Scooter.OngoingRentId.Value;
            return await _rentRepository
                .GetById(rentId)
                .ThenRequire(rent => rent.Stop(new(GetStopReason(ev.Scooter), _timestampProvider.Now)))
                .ThenIfSuccess(_rentRepository.Save)
                .ThenToResponse();
        }

        private RentStopReason GetStopReason(Scooter scooter) => scooter switch
        {
            { IsInStandby: true } => RentStopReason.Standby,
            { IsEnabled: false } => RentStopReason.Disabled,
            { IsOutOfService: true } => RentStopReason.OutOfArea,
            _ => RentStopReason.NotRentable
        };
    }
}
