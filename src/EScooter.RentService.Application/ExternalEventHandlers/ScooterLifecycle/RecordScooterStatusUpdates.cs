using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.ErrorManagement;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Application.ExternalEventHandlers.ScooterLifecycle
{
    public record ScooterStatusChanged(Guid Id, bool Standby) : ExternalEvent;

    public class RecordScooterStatusUpdates : ExternalEventHandlerBase<ScooterStatusChanged>
    {
        private readonly IScooterRepository _scooterRepository;

        public RecordScooterStatusUpdates(IScooterRepository scooterRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _scooterRepository = scooterRepository;
        }

        protected override async Task<Response<Nothing>> Handle(ScooterStatusChanged ev)
        {
            return await _scooterRepository.GetById(ev.Id)
                .ThenIfSuccess(scooter =>
                {
                    if (ev.Standby)
                    {
                        scooter.EnterStandby();
                    }
                    else
                    {
                        scooter.LeaveStandby();
                    }
                })
                .ThenIfSuccess(_scooterRepository.Save)
                .ThenToResponse();
        }
    }
}
