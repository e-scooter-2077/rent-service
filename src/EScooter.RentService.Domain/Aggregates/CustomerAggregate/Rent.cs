using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Represents the Rent of a scooter, holding state data about its lifecycle.
    /// </summary>
    public class Rent : Entity<Rent, Guid>.ExplicitId
    {
        /// <summary>
        /// Creates a new Rent from its properties.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The rent id.</param>
        /// <param name="scooterId">The scooter id.</param>
        /// <param name="confirmation">The confirmation info, empty if the rent was not confirmed.</param>
        /// <param name="cancellation">The cancellation info, empty if the rent was not cancelled.</param>
        /// <param name="end">The info about the end of the rent, empty if the rent is still ongoing.</param>
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

        /// <summary>
        /// The identifier of the rent.
        /// </summary>
        public override Guid Id { get; }

        /// <summary>
        /// The identifier of the scooter used by this rent.
        /// </summary>
        public Guid ScooterId { get; }

        /// <summary>
        /// Information about the confirmation of this rent.
        /// If empty, this indicates that the rent has not been confirmed yet.
        /// </summary>
        public Option<RentConfirmationInfo> Confirmation { get; private set; }

        /// <summary>
        /// Information about the cancellation of this rent.
        /// If empty, this indicates that the rent has not been cancelled.
        /// </summary>
        public Option<RentCancellationInfo> Cancellation { get; private set; }

        /// <summary>
        /// Information about the end of this rent.
        /// If empty, this indicates that the rent is still ongoing.
        /// </summary>
        public Option<RentEndInfo> End { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Rent"/> in the initial state for the scooter with the specified Id.
        /// </summary>
        /// <param name="scooterId">The scooter Id.</param>
        /// <returns>A new <see cref="Rent"/>.</returns>
        public static Rent CreateForScooter(Guid scooterId)
        {
            return new Rent(
                id: Guid.NewGuid(),
                scooterId: scooterId,
                confirmation: None,
                cancellation: None,
                end: None);
        }

        /// <summary>
        /// Confirms this <see cref="Rent"/>, recording its official starting time.
        /// </summary>
        /// <param name="timestamp">The confirmation instant.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Confirm(Timestamp timestamp)
        {
            return RequirePending()
                .IfSuccess(_ => Confirmation = new RentConfirmationInfo(timestamp));
        }

        /// <summary>
        /// Cancels this <see cref="Rent"/> before it is confirmed, specifying a reason for the cancellation.
        /// </summary>
        /// <param name="reason">The reason for the cancellation.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Cancel(RentCancellationReason reason)
        {
            return RequirePending()
                .IfSuccess(_ => Cancellation = new RentCancellationInfo(reason));
        }

        private Result<Nothing> RequirePending()
        {
            return RequireFalse(Cancellation.IsPresent, () => new RentAlreadyCancelled())
                .Require(_ => RequireFalse(Confirmation.IsPresent, () => new RentAlreadyConfirmed()));
        }
    }

    /// <summary>
    /// Represents an error used when an operation is made on a rent that has already been cancelled.
    /// </summary>
    public record RentAlreadyCancelled : DomainError;

    /// <summary>
    /// Represents an error used when an operation is made on a rent that has already been confirmed.
    /// </summary>
    public record RentAlreadyConfirmed : DomainError;

    /// <summary>
    /// Represents an error used when an operation is made on a rent that has already been ended.
    /// </summary>
    public record RentAlreadyEnded : DomainError;
}
