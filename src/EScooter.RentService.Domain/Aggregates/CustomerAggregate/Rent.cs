using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.Tools.Options;
using System;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    public class Rent : Entity<Rent, Guid>.ExplicitId
    {
        public Rent(
            Guid id,
            Guid scooterId,
            Option<RentConfirmationInfo> confirmation,
            Option<RentCancellationInfo> cancellation,
            Option<RentEndInfo> end)
        {
            Id = id;
            ScooterId = scooterId;
            Confirmation = confirmation;
            Cancellation = cancellation;
            End = end;
        }

        public override Guid Id { get; }

        public Guid ScooterId { get; }

        public Option<RentConfirmationInfo> Confirmation { get; private set; }

        public Option<RentCancellationInfo> Cancellation { get; private set; }

        public Option<RentEndInfo> End { get; private set; }
    }
}
