using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.Tools.Options;
using System;
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
    }
}
