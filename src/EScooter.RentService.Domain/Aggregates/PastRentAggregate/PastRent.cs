using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using System;

namespace EScooter.RentService.Domain.Aggregates.PastRentAggregate
{
    /// <summary>
    /// Represents a rent after its completion, either by a correct stop or by cancellation.
    /// </summary>
    public class PastRent : AggregateRoot
    {
        /// <summary>
        /// Creates a new <see cref="PastRent"/>.
        /// </summary>
        /// <param name="id">The rent Id.</param>
        /// <param name="scooterId">The scooter Id.</param>
        /// <param name="customerId">The customer Id.</param>
        /// <param name="requestTimestamp">The timestamp at which the rent was requested.</param>
        /// <param name="outcome">The outcome of the rent.</param>
        public PastRent(
            Guid id,
            Guid scooterId,
            Guid customerId,
            Timestamp requestTimestamp,
            RentOutcome outcome)
        {
            Id = id;
            ScooterId = scooterId;
            CustomerId = customerId;
            RequestTimestamp = requestTimestamp;
            Outcome = outcome;
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
        /// The outcome of the rent.
        /// </summary>
        public RentOutcome Outcome { get; }
    }
}
