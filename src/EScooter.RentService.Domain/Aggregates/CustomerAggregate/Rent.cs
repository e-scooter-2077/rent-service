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
        /// <param name="confirmationInfo">The confirmation info, empty if the rent was not confirmed.</param>
        /// <param name="cancellationInfo">The cancellation info, empty if the rent was not cancelled.</param>
        /// <param name="endInfo">The info about the end of the rent, empty if the rent is still ongoing.</param>
        public Rent(
            Guid id,
            Guid scooterId,
            Option<RentConfirmationInfo> confirmationInfo,
            Option<RentCancellationInfo> cancellationInfo,
            Option<RentEndInfo> endInfo)
        {
            Id = id;
            ScooterId = scooterId;
            ConfirmationInfo = confirmationInfo;
            CancellationInfo = cancellationInfo;
            EndInfo = endInfo;
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
        public Option<RentConfirmationInfo> ConfirmationInfo { get; private set; }

        /// <summary>
        /// Information about the cancellation of this rent.
        /// If empty, this indicates that the rent has not been cancelled.
        /// </summary>
        public Option<RentCancellationInfo> CancellationInfo { get; private set; }

        /// <summary>
        /// Information about the end of this rent.
        /// If empty, this indicates that the rent is still ongoing.
        /// </summary>
        public Option<RentEndInfo> EndInfo { get; private set; }

        private bool IsConfirmed => ConfirmationInfo.IsPresent;

        private bool IsCancelled => CancellationInfo.IsPresent;

        private bool IsEnded => EndInfo.IsPresent;

        private bool IsPending => !IsConfirmed && !IsCancelled;

        private bool IsOngoing => IsConfirmed && !IsEnded;

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
                confirmationInfo: None,
                cancellationInfo: None,
                endInfo: None);
        }

        /// <summary>
        /// Confirms this <see cref="Rent"/>, recording its official starting time.
        /// This operations fails with a <see cref="RentNotPending"/> error if this rent is not in a Pending state,
        /// in other words if it is confirmed or cancelled.
        /// </summary>
        /// <param name="timestamp">The confirmation instant.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Confirm(Timestamp timestamp)
        {
            return RequirePending()
                .IfSuccess(_ => ConfirmationInfo = new RentConfirmationInfo(timestamp));
        }

        /// <summary>
        /// Cancels this <see cref="Rent"/> before it is confirmed, specifying a reason for the cancellation.
        /// This operations fails with a <see cref="RentNotPending"/> error if this rent is not in a Pending state,
        /// in other words if it is confirmed or cancelled.
        /// </summary>
        /// <param name="reason">The reason for the cancellation.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Cancel(RentCancellationReason reason)
        {
            return RequirePending()
                .IfSuccess(_ => CancellationInfo = new RentCancellationInfo(reason));
        }

        private Result<Nothing> RequirePending() => RequireTrue(IsPending, () => new RentNotPending());

        /// <summary>
        /// Ends this <see cref="Rent"/> after it was confirmed, specifying a reason for the ending.
        /// This operation fails with a <see cref="RentNotOngoing"/> error if this rent is not in an Ongoing state,
        /// in other words if it is not confirmed or if it has already ended.
        /// </summary>
        /// <param name="reason">The reason for ending this rent.</param>
        /// <param name="timestamp">The ending instant.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> End(RentEndReason reason, Timestamp timestamp)
        {
            return RequireOngoing()
                .IfSuccess(_ => EndInfo = new RentEndInfo(reason, timestamp));
        }

        private Result<Nothing> RequireOngoing()
        {
            return RequireTrue(IsOngoing, () => new RentNotOngoing());
        }
    }

    /// <summary>
    /// Represents an error used when an operation is made on a rent that was required to be in a
    /// pending state, but was not.
    /// </summary>
    public record RentNotPending : DomainError;

    /// <summary>
    /// Represents an error used when an operation is made on a rent that was required to be in an
    /// ongoing state, but was not.
    /// </summary>
    public record RentNotOngoing : DomainError;
}
