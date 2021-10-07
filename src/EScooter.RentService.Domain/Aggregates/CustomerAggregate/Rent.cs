﻿using EasyDesk.CleanArchitecture.Domain.Metamodel;
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
        public Rent(
            Guid id,
            Guid scooterId,
            Option<RentConfirmationInfo> confirmationInfo)
        {
            Id = id;
            ScooterId = scooterId;
            ConfirmationInfo = confirmationInfo;
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
        /// Indicates whether this rent has been confirmed.
        /// </summary>
        public bool IsConfirmed => ConfirmationInfo.IsPresent;

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
                confirmationInfo: None);
        }

        /// <summary>
        /// Confirms this <see cref="Rent"/>, marking its actual start time.
        /// This operations fails with a <see cref="RentAlreadyConfirmed"/> error if this rent has already been confirmed.
        /// </summary>
        /// <param name="timestamp">The confirmation instant.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Confirm(Timestamp timestamp)
        {
            return RequireNotConfirmed()
                .IfSuccess(_ => ConfirmationInfo = new RentConfirmationInfo(timestamp));
        }

        private Result<Nothing> RequireNotConfirmed() => RequireFalse(IsConfirmed, () => new RentAlreadyConfirmed());
    }

    /// <summary>
    /// Represents an error used when trying to confirm a rent that has already been confirmed.
    /// </summary>
    public record RentAlreadyConfirmed : DomainError;
}
