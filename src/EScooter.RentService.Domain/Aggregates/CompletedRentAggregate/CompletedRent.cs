using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;

namespace EScooter.RentService.Domain.Aggregates.CompletedRentAggregate
{
    public class CompletedRent : AggregateRoot<CompletedRent>
    {
        public CompletedRent(
            Guid id,
            Guid scooterId,
            Option<RentConfirmationInfo> confirmationInfo,
            Option<RentCancellationInfo> cancellationInfo,
            Option<RentStopInfo> stopInfo) : base(id)
        {
            ScooterId = scooterId;
            ConfirmationInfo = confirmationInfo;
            CancellationInfo = cancellationInfo;
            StopInfo = stopInfo;
        }

        public Guid ScooterId { get; }

        public Option<RentConfirmationInfo> ConfirmationInfo { get; }

        public Option<RentCancellationInfo> CancellationInfo { get; }

        public Option<RentStopInfo> StopInfo { get; }
    }
}
