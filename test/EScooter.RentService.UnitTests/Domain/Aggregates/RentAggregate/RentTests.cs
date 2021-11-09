using System;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Testing;
using EasyDesk.CleanArchitecture.Testing.Domain;
using EasyDesk.Tools;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using Shouldly;
using Xunit;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.RentAggregate
{
    public class RentTests
    {
        private readonly Guid _scooterId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly SettableTimestampProvider _timestampProvider = new(Timestamp.Now);
        private readonly Rent _sut;

        public RentTests()
        {
            _sut = Rent.Create(_scooterId, _customerId, RequestTimestamp);
        }

        private Timestamp RequestTimestamp { get; } = Timestamp.Now;

        private Result<Nothing> DoAfterABit(Func<Timestamp, Result<Nothing>> action)
        {
            _timestampProvider.AdvanceBySeconds(1);
            return action(_timestampProvider.Now);
        }

        private Result<Nothing> Confirm() =>
            DoAfterABit(time => _sut.Confirm(new(time)));

        private Result<Nothing> Cancel(RentCancellationReason reason = RentCancellationReason.CreditInsufficient) =>
            DoAfterABit(time => _sut.Cancel(new(reason)));

        private Result<Nothing> Stop(RentStopReason reason = RentStopReason.StoppedByCustomer) =>
            DoAfterABit(time => _sut.Stop(new(reason, time)));

        [Fact]
        public void Create_ShouldReturnARentInTheInitialState()
        {
            _sut.ShouldSatisfyAllConditions(
                rent => rent.ScooterId.ShouldBe(_scooterId),
                rent => rent.ConfirmationInfo.ShouldBeEmpty(),
                rent => rent.CancellationInfo.ShouldBeEmpty(),
                rent => rent.StopInfo.ShouldBeEmpty(),
                rent => rent.RequestTimestamp.ShouldBe(RequestTimestamp));
        }

        [Fact]
        public void Create_ShouldEmitAnEvent()
        {
            _sut.ShouldHaveEmitted(new RentRequestedEvent(_sut));
        }

        [Fact]
        public void State_ShouldBePending_ImmediatlyAfterCreation()
        {
            _sut.State.ShouldBe(RentState.Pending);
        }

        [Fact]
        public void State_ShouldBeOngoing_IfTheRentIsConfirmed()
        {
            Confirm();

            _sut.State.ShouldBe(RentState.Ongoing);
        }

        [Fact]
        public void State_ShouldBeCancelled_IfTheRentIsCancelledBeforeConfirmation()
        {
            Cancel();

            _sut.State.ShouldBe(RentState.Cancelled);
        }

        [Fact]
        public void State_ShouldBeCancelled_IfTheRentIsCancelledAfterConfirmation()
        {
            Confirm();
            Cancel();

            _sut.State.ShouldBe(RentState.Cancelled);
        }

        [Fact]
        public void State_ShouldBeCompleted_IfTheRentIsStopped()
        {
            Confirm();
            Stop();

            _sut.State.ShouldBe(RentState.Completed);
        }

        [Fact]
        public void Confirm_ShouldSucceed_IfTheRentIsPending()
        {
            Confirm().ShouldBe(Ok);
        }

        [Fact]
        public void Confirm_ShouldFillTheRentWithConfirmationInfo_IfTheRentIsPending()
        {
            Confirm();

            _sut.ConfirmationInfo.ShouldContain(new RentConfirmationInfo(_timestampProvider.Now));
        }

        [Fact]
        public void Confirm_ShouldEmitAnEvent_IfTheRentIsPending()
        {
            Confirm();

            _sut.ShouldHaveEmitted(new RentConfirmedEvent(_sut, new RentConfirmationInfo(_timestampProvider.Now)));
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentIsOngoing()
        {
            Confirm();

            Confirm().ShouldBe(new InvalidRentState(RentState.Ongoing));
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentIsCancelled()
        {
            Cancel();

            Confirm().ShouldBe(new InvalidRentState(RentState.Cancelled));
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentIsCompleted()
        {
            Confirm();
            Stop();

            Confirm().ShouldBe(new InvalidRentState(RentState.Completed));
        }

        [Fact]
        public void Cancel_ShouldSucceed_IfTheRentIsPending()
        {
            Cancel().ShouldBe(Ok);
        }

        [Fact]
        public void Cancel_ShouldFillTheRentWithCancellationInfo_IfTheRentIsPending()
        {
            var reason = RentCancellationReason.CreditInsufficient;
            Cancel(reason);

            _sut.CancellationInfo.ShouldContain(new RentCancellationInfo(reason));
        }

        [Fact]
        public void Cancel_ShouldEmitAnEvent_IfTheRentIsPending()
        {
            var reason = RentCancellationReason.CreditInsufficient;
            Cancel(reason);

            _sut.ShouldHaveEmitted(new RentCancelledEvent(_sut, new(reason)));
            _sut.ShouldHaveEmitted(new RentEndedEvent(_sut));
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentIsCancelled()
        {
            Cancel();

            Cancel().ShouldBe(new InvalidRentState(RentState.Cancelled));
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentIsCompleted()
        {
            Confirm();
            Stop();

            Cancel().ShouldBe(new InvalidRentState(RentState.Completed));
        }

        [Fact]
        public void Stop_ShouldSucceed_IfTheRentIsOngoing()
        {
            Confirm();

            Stop().ShouldBe(Ok);
        }

        [Fact]
        public void Stop_ShouldFillTheRentWithStopInfo_IfTheRentIsOngoing()
        {
            Confirm();
            var reason = RentStopReason.StoppedByCustomer;
            Stop(reason);

            _sut.StopInfo.ShouldContain(new RentStopInfo(reason, _timestampProvider.Now));
        }

        [Fact]
        public void Stop_ShouldEmitAnEvent_IfTheRentIsOngoing()
        {
            Confirm();
            var reason = RentStopReason.StoppedByCustomer;
            Stop(reason);

            _sut.ShouldHaveEmitted(new RentStoppedEvent(_sut, new(reason, _timestampProvider.Now)));
            _sut.ShouldHaveEmitted(new RentEndedEvent(_sut));
        }

        [Fact]
        public void Stop_ShouldFail_IfTheRentIsPending()
        {
            Stop().ShouldBe(new InvalidRentState(RentState.Pending));
        }

        [Fact]
        public void Stop_ShouldFail_IfTheRentIsCancelled()
        {
            Cancel();

            Stop().ShouldBe(new InvalidRentState(RentState.Cancelled));
        }

        [Fact]
        public void Stop_ShouldFail_IfTheRentIsCompleted()
        {
            Confirm();
            Stop();

            Stop().ShouldBe(new InvalidRentState(RentState.Completed));
        }
    }
}
