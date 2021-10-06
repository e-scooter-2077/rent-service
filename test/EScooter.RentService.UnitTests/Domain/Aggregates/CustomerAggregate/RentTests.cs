using System;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using Shouldly;
using Xunit;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.CustomerAggregate
{
    public class RentTests
    {
        private const RentCancellationReason CancellationReason = RentCancellationReason.CreditInsufficient;
        private const RentEndReason EndReason = RentEndReason.StoppedByCustomer;

        private readonly Timestamp _timestamp1 = Timestamp.Now;
        private readonly Timestamp _timestamp2 = Timestamp.Now + TimeOffset.FromMinutes(1);
        private readonly Guid _scooterId = Guid.NewGuid();
        private readonly Rent _sut;

        public RentTests()
        {
            _sut = Rent.CreateForScooter(_scooterId);
        }

        [Fact]
        public void CreateForScooter_ShouldReturnARentInTheInitialState()
        {
            _sut.ScooterId.ShouldBe(_scooterId);
            _sut.ConfirmationInfo.ShouldBeEmpty();
            _sut.CancellationInfo.ShouldBeEmpty();
            _sut.EndInfo.ShouldBeEmpty();
        }

        [Fact]
        public void Confirm_ShouldFillTheRentWithConfirmationInfo_IfTheRentIsPending()
        {
            _sut.Confirm(_timestamp1);

            _sut.ConfirmationInfo.ShouldContain(new RentConfirmationInfo(_timestamp1));
        }

        [Fact]
        public void Confirm_ShouldReturnOk_IfTheRentIsPending()
        {
            _sut.Confirm(_timestamp1).ShouldBe(Ok);
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(_timestamp1);

            _sut.Confirm(_timestamp2).ShouldBe(new RentNotPending());
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenCancelled()
        {
            _sut.Cancel(CancellationReason);

            _sut.Confirm(_timestamp1).ShouldBe(new RentNotPending());
        }

        [Fact]
        public void Cancel_ShouldReturnOk_IfTheRentIsPending()
        {
            _sut.Cancel(CancellationReason).ShouldBe(Ok);
        }

        [Fact]
        public void Cancel_ShouldFillTheRentWithCancellationInfo_IfTheRentIsPending()
        {
            _sut.Cancel(CancellationReason);

            _sut.CancellationInfo.ShouldContain(new RentCancellationInfo(CancellationReason));
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(_timestamp1);

            _sut.Cancel(CancellationReason).ShouldBe(new RentNotPending());
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentHasAlreadyBeenCancelled()
        {
            _sut.Cancel(CancellationReason);

            _sut.Cancel(CancellationReason).ShouldBe(new RentNotPending());
        }

        [Fact]
        public void End_ShouldReturnOk_IfTheRentIsOngoing()
        {
            _sut.Confirm(_timestamp1);

            _sut.End(EndReason, _timestamp2).ShouldBe(Ok);
        }

        [Fact]
        public void End_ShouldFillTheRentWithEndInfo_IfTheRentIsOngoing()
        {
            _sut.Confirm(_timestamp1);

            _sut.End(EndReason, _timestamp2);

            _sut.EndInfo.ShouldContain(new RentEndInfo(EndReason, _timestamp2));
        }

        [Fact]
        public void End_ShouldFail_IfTheRentHasNotBeenConfirmed()
        {
            _sut.End(EndReason, _timestamp1).ShouldBe(new RentNotOngoing());
        }

        [Fact]
        public void End_ShouldFail_IfTheRentHasBeenCancelled()
        {
            _sut.Cancel(CancellationReason);

            _sut.End(EndReason, _timestamp1).ShouldBe(new RentNotOngoing());
        }

        [Fact]
        public void End_ShouldFail_IfTheRentHasAlreadyEnded()
        {
            _sut.Confirm(_timestamp1);
            _sut.End(EndReason, _timestamp2);

            _sut.End(EndReason, _timestamp2 + TimeOffset.FromSeconds(1)).ShouldBe(new RentNotOngoing());
        }
    }
}
