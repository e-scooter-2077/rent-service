using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Domain.Utils;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Represents a customer within the Rent Context, that is entity able to perform <see cref="Rent"/>s.
    /// </summary>
    public class Customer : AggregateRoot<Customer>
    {
        /// <summary>
        /// Creates a new <see cref="Customer"/>.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <param name="ongoingRent">The optional ongoing <see cref="Rent"/>.</param>
        public Customer(Guid id, Option<Rent> ongoingRent) : base(id)
        {
            OngoingRent = ongoingRent;
        }

        /// <summary>
        /// Returns the currently active <see cref="Rent"/> for this customer, if any.
        /// </summary>
        public Option<Rent> OngoingRent { get; private set; }

        private bool HasOngoingRent => OngoingRent.IsPresent;

        /// <summary>
        /// Creates a new <see cref="Customer"/> given its identifier.
        /// </summary>
        /// <param name="id">The customer Id.</param>
        /// <returns>The new <see cref="Customer"/>.</returns>
        public static Customer Create(Guid id) => new(id, ongoingRent: None);

        /// <summary>
        /// Requests for this customer a new <see cref="Rent"/> for the given scooter.
        /// This operation fails with a <see cref="RentAlreadyOngoing"/> error if a rent is ongoing.
        /// </summary>
        /// <param name="scooterId">The scooter Id.</param>
        /// <param name="requestTimestamp">The instant in which the rent is requested.</param>
        /// <returns>A result containing the requested <see cref="Rent"/> or an error.</returns>
        public Result<Rent> RequestRent(Guid scooterId, Timestamp requestTimestamp)
        {
            if (HasOngoingRent)
            {
                return new RentAlreadyOngoing();
            }
            var rent = Rent.CreateForScooter(scooterId, requestTimestamp);
            OngoingRent = rent;
            EmitEvent(new RentRequestedEvent(this, rent));
            return rent;
        }

        /// <summary>
        /// Confirms the ongoing <see cref="Rent"/>.
        /// This operation fails with a <see cref="NoOngoingRent"/> error if no rent is ongoing.
        /// </summary>
        /// <param name="confirmationInfo">The information about how to confirm the rent.</param>
        /// <returns>A result containing the confirmed <see cref="Rent"/> or an error.</returns>
        public Result<Rent> ConfirmOngoingRent(RentConfirmationInfo confirmationInfo)
        {
            return RequireOngoingRent()
                .Require(rent => rent.Confirm(confirmationInfo))
                .IfSuccess(rent => EmitEvent(new RentConfirmedEvent(this, rent, confirmationInfo)));
        }

        /// <summary>
        /// Cancels the ongoing <see cref="Rent"/> for the given reason.
        /// This operation fails with a <see cref="NoOngoingRent"/> error if no rent is ongoing.
        /// </summary>
        /// <param name="cancellationInfo">The information about how to cancel the rent.</param>
        /// <returns>A result containing the cancelled <see cref="Rent"/> or an error.</returns>
        public Result<Rent> CancelOngoingRent(RentCancellationInfo cancellationInfo)
        {
            return RequireOngoingRent()
                .IfSuccess(rent =>
                {
                    CompleteOngoingRent();
                    EmitEvent(new RentCancelledEvent(this, rent, cancellationInfo));
                });
        }

        /// <summary>
        /// Stops the ongoing <see cref="Rent"/> for the given reason, marking its actual stopping time.
        /// </summary>
        /// <param name="stopInfo">The information about how to stop the rent.</param>
        /// <returns>A result containing the stopped <see cref="Rent"/> or an error.</returns>
        public Result<Rent> StopOngoingRent(RentStopInfo stopInfo)
        {
            return RequireOngoingRent()
                .Require(rent => RequireTrue(rent.IsConfirmed, () => new RentNotConfirmed()))
                .IfSuccess(rent =>
                {
                    CompleteOngoingRent();
                    EmitEvent(new RentStoppedEvent(this, rent, stopInfo));
                });
        }

        private Result<Rent> RequireOngoingRent() => OngoingRent.OrElseError(() => new NoOngoingRent());

        private void CompleteOngoingRent() => OngoingRent = None;
    }

    /// <summary>
    /// An event emitted when a customer requests a new rent.
    /// </summary>
    /// <param name="Customer">The customer requesting the rent.</param>
    /// <param name="Rent">The requested rent.</param>
    public record RentRequestedEvent(Customer Customer, Rent Rent) : IDomainEvent;

    /// <summary>
    /// An event emitted when a rent is confirmed.
    /// </summary>
    /// <param name="Customer">The customer for which the rent was confirmed.</param>
    /// <param name="Rent">The confirmed rent.</param>
    /// <param name="ConfirmationInfo">The information about how the rent was confirmed.</param>
    public record RentConfirmedEvent(Customer Customer, Rent Rent, RentConfirmationInfo ConfirmationInfo) : IDomainEvent;

    /// <summary>
    /// An event emitted when a rent is cancelled.
    /// </summary>
    /// <param name="Customer">The customer for which the rent was cancelled.</param>
    /// <param name="Rent">The cancelled rent.</param>
    /// <param name="CancellationInfo">The information about how the rent was cancelled.</param>
    public record RentCancelledEvent(Customer Customer, Rent Rent, RentCancellationInfo CancellationInfo) : IDomainEvent;

    /// <summary>
    /// An event emitted when a rent is stopped.
    /// </summary>
    /// <param name="Customer">The customer for which the rent was stopped.</param>
    /// <param name="Rent">The stopped rent.</param>
    /// <param name="StopInfo">The information about how the rent was stopped.</param>
    public record RentStoppedEvent(Customer Customer, Rent Rent, RentStopInfo StopInfo) : IDomainEvent;

    /// <summary>
    /// An error returned when trying to request a rent while another one is still ongoing.
    /// </summary>
    public record RentAlreadyOngoing : DomainError;

    /// <summary>
    /// An error returned when trying to stop a rent before it is confirmed.
    /// </summary>
    public record RentNotConfirmed : DomainError;

    /// <summary>
    /// An error returned when trying to perform an action on the ongoing rent while not having one.
    /// </summary>
    public record NoOngoingRent : DomainError;
}
