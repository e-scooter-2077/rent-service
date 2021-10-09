using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using System;
using static EasyDesk.Tools.Functions;

namespace EScooter.RentService.Domain.Aggregates.PastRentAggregate
{
    /// <summary>
    /// Represents one of the possible ways in which a rent can end, either by correct completion or by cancellation.
    /// </summary>
    public abstract record RentOutcome
    {
        private record CompletedOutcome(RentConfirmationInfo ConfirmationInfo, RentStopInfo StopInfo) : RentOutcome;

        private record CancelledOutcome(RentCancellationInfo CancellationInfo) : RentOutcome;

        private RentOutcome()
        {
        }

        /// <summary>
        /// Creates a new Completed outcome.
        /// </summary>
        /// <param name="confirmationInfo">The information about how the rent was confirmed.</param>
        /// <param name="stopInfo">The information about how the rent was stopped.</param>
        /// <returns>A new <see cref="RentOutcome"/>.</returns>
        public static RentOutcome Completed(RentConfirmationInfo confirmationInfo, RentStopInfo stopInfo) =>
            new CompletedOutcome(confirmationInfo, stopInfo);

        /// <summary>
        /// Creates a new Cancelled outcome.
        /// </summary>
        /// <param name="cancellationInfo">The information about how the rent was cancelled.</param>
        /// <returns>A new <see cref="RentOutcome"/>.</returns>
        public static RentOutcome Cancelled(RentCancellationInfo cancellationInfo) =>
            new CancelledOutcome(cancellationInfo);

        /// <summary>
        /// Matches this outcome against all possible outcomes, returning a different value depending on the matching outcome.
        /// </summary>
        /// <typeparam name="T">The type of the returned value.</typeparam>
        /// <param name="completed">What to return if the outcome is "Completed".</param>
        /// <param name="cancelled">What to return if the outcome is "Cancelled".</param>
        /// <returns>The value returned by the matching path.</returns>
        public T Match<T>(Func<RentConfirmationInfo, RentStopInfo, T> completed, Func<RentCancellationInfo, T> cancelled) => this switch
        {
            CompletedOutcome(var confirmation, var stop) => completed(confirmation, stop),
            CancelledOutcome(var cancellation) => cancelled(cancellation),
            _ => throw new InvalidOperationException()
        };

        /// <summary>
        /// Matches this outcome against all possible outcomes, performing a different action depending on the matching outcome.
        /// </summary>
        /// <param name="completed">What to do if the outcome is "Completed".</param>
        /// <param name="cancelled">What to do if the outcome is "Cancelled".</param>
        public void Match(Action<RentConfirmationInfo, RentStopInfo> completed = default, Action<RentCancellationInfo> cancelled = default) => Match(
            completed: (c, s) => JustDoIt(() => completed?.Invoke(c, s)),
            cancelled: c => JustDoIt(() => cancelled?.Invoke(c)));
    }
}
