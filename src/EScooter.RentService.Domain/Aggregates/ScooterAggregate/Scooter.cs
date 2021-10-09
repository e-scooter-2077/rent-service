using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EasyDesk.Tools.Options;
using System;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.Domain.Aggregates.ScooterAggregate
{
    /// <summary>
    /// Represents a scooter within the Rent Context, holding state about its rentability at any moment.
    /// </summary>
    public class Scooter : AggregateRoot<Scooter>
    {
        /// <summary>
        /// Create a new <see cref="Scooter"/>.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The scooter Id.</param>
        /// <param name="isRentable">The rentability state.</param>
        /// <param name="rentingCustomerId">The Id of the customer currently renting this scooter, if any.</param>
        public Scooter(Guid id, bool isRentable, Option<Guid> rentingCustomerId) : base(id)
        {
            IsRentable = isRentable;
            RentingCustomerId = rentingCustomerId;
        }

        /// <summary>
        /// Indicates whether customers are currently authorized to rent this scooter.
        /// </summary>
        public bool IsRentable { get; private set; }

        /// <summary>
        /// The identifier of the customer currently renting this scooter.
        /// If empty, this means that this scooter is currently available for rent.
        /// </summary>
        public Option<Guid> RentingCustomerId { get; private set; }

        private bool IsAvailable => RentingCustomerId.IsAbsent;

        /// <summary>
        /// Marks this scooter as rented by the customer with the given Id.
        /// This operation fails with a <see cref="NotRentable"/> error if this scooter is currently not rentable,
        /// or with an <see cref="AlreadyRented"/> error if it is already rented by a customer.
        /// </summary>
        /// <param name="customerId">The customer Id.</param>
        /// <returns>A result that indicates whether the operation was successful.</returns>
        public Result<Nothing> Rent(Guid customerId)
        {
            return RequireRentablility()
                .Require(_ => RequireAvailability())
                .IfSuccess(_ =>
                {
                    RentingCustomerId = customerId;
                });
        }

        private Result<Nothing> RequireRentablility() => RequireTrue(IsRentable, () => new NotRentable());

        private Result<Nothing> RequireAvailability() => RequireTrue(IsAvailable, () => new AlreadyRented());

        /// <summary>
        /// Marks this scooter as not rented by any customer. If this scooter was already not rented, the scooter
        /// is left unchanged.
        /// </summary>
        public void ClearRent() => RentingCustomerId = None;

        /// <summary>
        /// Allows customers to rent this scooter.
        /// </summary>
        public void MakeRentable() => IsRentable = true;

        /// <summary>
        /// Disallows customers to rent this scooter.
        /// </summary>
        public void MakeNotRentable() => IsRentable = false;
    }

    /// <summary>
    /// An error returned when trying to rent a scooter that is not rentable.
    /// </summary>
    public record NotRentable : DomainError;

    /// <summary>
    /// An error returned when trying to rent a scooter that is already rented by a customer.
    /// </summary>
    public record AlreadyRented : DomainError;
}
