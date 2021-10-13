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
    public class Scooter : AggregateRoot
    {
        /// <summary>
        /// Create a new <see cref="Scooter"/>.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The scooter Id.</param>
        /// <param name="rentingCustomerId">The Id of the customer currently renting this scooter, if any.</param>
        /// <param name="isEnabled">The enabled state.</param>
        /// <param name="isOutOfService">The out-of-service state.</param>
        /// <param name="isInStandby">The standby state.</param>
        public Scooter(
            Guid id,
            Option<Guid> rentingCustomerId,
            bool isEnabled,
            bool isOutOfService,
            bool isInStandby)
        {
            Id = id;
            RentingCustomerId = rentingCustomerId;
            IsEnabled = isEnabled;
            IsOutOfService = isOutOfService;
            IsInStandby = isInStandby;
        }

        /// <summary>
        /// The unique identifier of this scooter.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Indicates whether this scooter is enabled, due to a maintainer's choice.
        /// A scooter cannot be rented as long as it is disabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Indicates whether this scooter is out of service, due to the scooter leaving its area of service.
        /// A scooter cannot be rented as long as it is out of service.
        /// </summary>
        public bool IsOutOfService { get; private set; }

        /// <summary>
        /// Indicates whether this scooter is in standby mode, due to the scooter's battery falling below the minimum level.
        /// A scooter cannot be rented as long as it is in standby.
        /// </summary>
        public bool IsInStandby { get; private set; }

        /// <summary>
        /// The identifier of the customer currently renting this scooter.
        /// If empty, this means that this scooter is currently available for rent.
        /// </summary>
        public Option<Guid> RentingCustomerId { get; private set; }

        private bool IsAvailable => RentingCustomerId.IsAbsent;

        private bool IsRentable => IsEnabled && !IsOutOfService && !IsInStandby;

        private Result<Nothing> RequireRentablility() => RequireTrue(IsRentable, () => new NotRentable());

        private Result<Nothing> RequireAvailability() => RequireTrue(IsAvailable, () => new AlreadyRented());

        /// <summary>
        /// Creates a new scooter in its default state.
        /// </summary>
        /// <param name="id">The scooter Id.</param>
        /// <returns>A new <see cref="Scooter"/>.</returns>
        public static Scooter Create(Guid id)
        {
            return new(
                id: id,
                rentingCustomerId: None,
                isEnabled: true,
                isInStandby: false,
                isOutOfService: false);
        }

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
                .IfSuccess(_ => RentingCustomerId = customerId);
        }

        /// <summary>
        /// Marks this scooter as not rented by any customer. If this scooter was already not rented, the scooter
        /// is left unchanged.
        /// </summary>
        public void ClearRent() => RentingCustomerId = None;

        /// <summary>
        /// Enables this scooter.
        /// </summary>
        public void Enable() => SetEnabled(enabled: true);

        /// <summary>
        /// Disables this scooter.
        /// </summary>
        public void Disable() => SetEnabled(enabled: false);

        private void SetEnabled(bool enabled) => IsEnabled = enabled;

        /// <summary>
        /// Makes this scooter enter standby mode.
        /// </summary>
        public void EnterStandby() => SetStandby(standby: true);

        /// <summary>
        /// Makes this scooter leave standby mode.
        /// </summary>
        public void LeaveStandby() => SetStandby(standby: false);

        private void SetStandby(bool standby) => IsInStandby = standby;

        /// <summary>
        /// Marks this scooter as being inside an area of service.
        /// </summary>
        public void EnterAreaOfService() => SetOutOfService(outOfService: false);

        /// <summary>
        /// Marks this scooter as not being inside an area of service.
        /// </summary>
        public void LeaveAreaOfService() => SetOutOfService(outOfService: true);

        private void SetOutOfService(bool outOfService) => IsOutOfService = outOfService;
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
