using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;
using System.Linq;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.Domain.Aggregates.RentAggregate
{
    /// <summary>
    /// Represents a rent during its lifecycle.
    /// </summary>
    public class Rent : AggregateRoot
    {
        /// <summary>
        /// Creates a new <see cref="Rent"/>.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The rent Id.</param>
        /// <param name="scooterId">The scooter Id.</param>
        /// <param name="customerId">The customer Id.</param>
        /// <param name="requestTimestamp">The timestamp at which the rent was requested.</param>
        /// <param name="confirmationInfo">The confirmation info.</param>
        /// <param name="cancellationInfo">The cancellation info.</param>
        /// <param name="stopInfo">The stop info.</param>
        public Rent(
            Guid id,
            Guid scooterId,
            Guid customerId,
            Timestamp requestTimestamp,
            Option<RentConfirmationInfo> confirmationInfo,
            Option<RentCancellationInfo> cancellationInfo,
            Option<RentStopInfo> stopInfo)
        {
            Id = id;
            ScooterId = scooterId;
            CustomerId = customerId;
            RequestTimestamp = requestTimestamp;
            ConfirmationInfo = confirmationInfo;
            CancellationInfo = cancellationInfo;
            StopInfo = stopInfo;
        }

        /// <summary>
        /// The unique identifier of this rent.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The identifier of the scooter used by this rent.
        /// </summary>
        public Guid ScooterId { get; }

        /// <summary>
        /// The identifier of the customer that requested this rent.
        /// </summary>
        public Guid CustomerId { get; }

        /// <summary>
        /// The instant at which this rent was requested.
        /// </summary>
        public Timestamp RequestTimestamp { get; }

        /// <summary>
        /// Returns the current state of this rent.
        /// </summary>
        public RentState State
        {
            get
            {
                if (StopInfo.IsPresent)
                {
                    return RentState.Completed;
                }
                else if (CancellationInfo.IsPresent)
                {
                    return RentState.Cancelled;
                }
                else if (ConfirmationInfo.IsPresent)
                {
                    return RentState.Ongoing;
                }
                else
                {
                    return RentState.Pending;
                }
            }
        }

        /// <summary>
        /// Returns the information about how this rent was confirmed.
        /// </summary>
        public Option<RentConfirmationInfo> ConfirmationInfo { get; private set; }

        /// <summary>
        /// Returns the information about how this rent was cancelled.
        /// </summary>
        public Option<RentCancellationInfo> CancellationInfo { get; private set; }

        /// <summary>
        /// Returns the information about how this rent was stopped.
        /// </summary>
        public Option<RentStopInfo> StopInfo { get; private set; }

        /// <summary>
        /// Creates a new rent for the given scooter and customer.
        /// </summary>
        /// <param name="scooterId">The scooter Id.</param>
        /// <param name="customerId">The customer Id.</param>
        /// <param name="requestTimestamp">The timestamp of the request.</param>
        /// <returns>The created rent.</returns>
        public static Rent Create(Guid scooterId, Guid customerId, Timestamp requestTimestamp)
        {
            var rent = new Rent(
                id: Guid.NewGuid(),
                scooterId: scooterId,
                customerId: customerId,
                requestTimestamp: requestTimestamp,
                confirmationInfo: None,
                cancellationInfo: None,
                stopInfo: None);
            rent.EmitEvent(new RentRequestedEvent(rent));
            return rent;
        }

        /// <summary>
        /// Confirms this rent.
        /// </summary>
        /// <param name="confirmationInfo">Information about how this rent should be confirmed.</param>
        /// <returns>
        /// <para>
        ///     <see cref="InvalidRentState"/>: if this rent has already been confirmed or cancelled.
        /// </para>
        /// <para>
        ///     <see cref="Ok"/>: otherwise.
        /// </para>
        /// </returns>
        public Result<Nothing> Confirm(RentConfirmationInfo confirmationInfo)
        {
            return RequireState(RentState.Pending)
                .IfSuccess(_ =>
                {
                    ConfirmationInfo = confirmationInfo;
                    EmitEvent(new RentConfirmedEvent(this, confirmationInfo));
                });
        }

        /// <summary>
        /// Cancels this rent.
        /// </summary>
        /// <param name="cancellationInfo">Information about how this rent should be cancelled.</param>
        /// <returns>
        /// <para>
        ///     <see cref="InvalidRentState"/>: if this rent has already been cancelled or stopped.
        /// </para>
        /// <para>
        ///     <see cref="Ok"/>: otherwise.
        /// </para>
        /// </returns>
        public Result<Nothing> Cancel(RentCancellationInfo cancellationInfo)
        {
            return RequireState(RentState.Pending, RentState.Ongoing)
                .IfSuccess(_ =>
                {
                    CancellationInfo = cancellationInfo;
                    EmitEvent(new RentCancelledEvent(this, cancellationInfo));
                    EmitEvent(new RentEndedEvent(this));
                });
        }

        /// <summary>
        /// Cancels this rent.
        /// </summary>
        /// <param name="stopInfo">Information about how this rent should be stopped.</param>
        /// <returns>
        /// <para>
        ///     <see cref="InvalidRentState"/>: if this rent has already been cancelled or stopped, or
        ///     if it hasn't been confirmed yet.
        /// </para>
        /// <para>
        ///     <see cref="Ok"/>: otherwise.
        /// </para>
        /// </returns>
        public Result<Nothing> Stop(RentStopInfo stopInfo)
        {
            return RequireState(RentState.Ongoing)
                .IfSuccess(_ =>
                {
                    StopInfo = stopInfo;
                    EmitEvent(new RentStoppedEvent(this, stopInfo));
                    EmitEvent(new RentEndedEvent(this));
                });
        }

        private Result<Nothing> RequireState(params RentState[] possibleStates)
        {
            var currentState = State;
            return RequireTrue(possibleStates.Contains(currentState), () => new InvalidRentState(currentState));
        }
    }

    /// <summary>
    /// An event emitted when a customer requests a new rent.
    /// </summary>
    /// <param name="Rent">The requested rent.</param>
    public record RentRequestedEvent(Rent Rent) : DomainEvent;

    /// <summary>
    /// An event emitted when a rent is confirmed.
    /// </summary>
    /// <param name="Rent">The confirmed rent.</param>
    /// <param name="ConfirmationInfo">The information about how the rent was confirmed.</param>
    public record RentConfirmedEvent(Rent Rent, RentConfirmationInfo ConfirmationInfo) : DomainEvent;

    /// <summary>
    /// An event emitted when a rent is cancelled.
    /// </summary>
    /// <param name="Rent">The cancelled rent.</param>
    /// <param name="CancellationInfo">The information about how the rent was cancelled.</param>
    public record RentCancelledEvent(Rent Rent, RentCancellationInfo CancellationInfo) : DomainEvent;

    /// <summary>
    /// An event emitted when a rent is stopped.
    /// </summary>
    /// <param name="Rent">The stopped rent.</param>
    /// <param name="StopInfo">The information about how the rent was stopped.</param>
    public record RentStoppedEvent(Rent Rent, RentStopInfo StopInfo) : DomainEvent;

    /// <summary>
    /// An event emitted when a rent ends, either by a stop or by cancellation.
    /// </summary>
    /// <param name="Rent">The ended rent.</param>
    public record RentEndedEvent(Rent Rent) : DomainEvent;

    /// <summary>
    /// An error returned when trying to perform an operation on a rent that is in an invalid state
    /// for that operation.
    /// </summary>
    /// <param name="CurrentState">The current state of the rent.</param>
    public record InvalidRentState(RentState CurrentState) : DomainError;
}
