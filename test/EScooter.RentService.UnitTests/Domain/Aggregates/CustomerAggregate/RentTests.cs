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

        private readonly Timestamp _timestamp = Timestamp.Now;
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
            _sut.Confirmation.ShouldBeEmpty();
            _sut.Cancellation.ShouldBeEmpty();
            _sut.End.ShouldBeEmpty();
        }

        [Fact]
        public void Confirm_ShouldFillTheRentWithConfirmationInfo()
        {
            _sut.Confirm(_timestamp);

            _sut.Confirmation.ShouldContain(new RentConfirmationInfo(_timestamp));
        }

        [Fact]
        public void Confirm_ShouldReturnOk_IfConfirmationIsSuccessful()
        {
            _sut.Confirm(_timestamp).ShouldBe(Ok);
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(_timestamp);

            _sut.Confirm(_timestamp + TimeOffset.FromMinutes(1)).ShouldBe(new RentAlreadyConfirmed());
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenCancelled()
        {
            _sut.Cancel(CancellationReason);

            _sut.Confirm(_timestamp).ShouldBe(new RentAlreadyCancelled());
        }

        [Fact]
        public void Cancel_ShouldReturnOk_IfCancellationIsSuccessful()
        {
            _sut.Cancel(CancellationReason).ShouldBe(Ok);
        }

        [Fact]
        public void Cancel_ShouldFillTheRentWithCancellationInfo()
        {
            _sut.Cancel(CancellationReason);

            _sut.Cancellation.ShouldContain(new RentCancellationInfo(CancellationReason));
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(_timestamp);

            _sut.Cancel(CancellationReason).ShouldBe(new RentAlreadyConfirmed());
        }

        [Fact]
        public void Cancel_ShouldFail_IfTheRentHasAlreadyBeenCancelled()
        {
            _sut.Cancel(CancellationReason);

            _sut.Cancel(CancellationReason).ShouldBe(new RentAlreadyCancelled());
        }
    }
}
