using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using Shouldly;
using System;
using Xunit;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.CustomerAggregate
{
    public class CustomerTests
    {
        private const RentCancellationReason CancellationReason = RentCancellationReason.InternalError;
        private const RentStopReason StopReason = RentStopReason.StoppedByCustomer;

        private readonly Customer _sut;
        private readonly Guid _scooterId = Guid.NewGuid();
        private readonly Timestamp _timestamp1 = Timestamp.Now;
        private readonly Timestamp _timestamp2 = Timestamp.Now + TimeOffset.FromMinutes(1);

        public CustomerTests()
        {
            _sut = Customer.Create(Guid.NewGuid());
        }

        [Fact]
        public void Create_ShouldCreateACustomerWithNoActiveRent()
        {
            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void RequestRent_ShouldReturnTheCreatedRent_IfTheCustomerHasNoOngoingRent()
        {
            _sut.RequestRent(_scooterId).ReadValue().ShouldSatisfyAllConditions(rent =>
            {
                rent.ScooterId.ShouldBe(_scooterId);
                rent.ConfirmationInfo.ShouldBeEmpty();
            });
        }

        [Fact]
        public void RequestRent_ShouldStartANewRent_IfTheCustomerHasNoOngoingRent()
        {
            var rent = _sut.RequestRent(_scooterId).ReadValue();

            _sut.OngoingRent.ShouldContain(rent);
        }

        [Fact]
        public void RequestRent_ShouldFail_IfTheCustomerAlreadyHasAnOngoingRent()
        {
            _sut.RequestRent(_scooterId);

            _sut.RequestRent(Guid.NewGuid()).ShouldBe(new RentAlreadyOngoing());
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldReturnTheOngoingRent_IfTheCustomerHasOne()
        {
            var rent = _sut.RequestRent(_scooterId).ReadValue();

            _sut.ConfirmOngoingRent(_timestamp1).ShouldBe(rent);
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldConfirmTheOngoingRent_IfTheCustomerHasOne()
        {
            _sut.RequestRent(_scooterId);

            _sut.ConfirmOngoingRent(_timestamp1);

            _sut.OngoingRent.Value.ConfirmationInfo.ShouldContain(new RentConfirmationInfo(_timestamp1));
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            _sut.ConfirmOngoingRent(_timestamp1).ShouldBe(new NoOngoingRent());
        }

        [Fact]
        public void CancelOngoingRent_ShouldRemoveTheOngoingRent_IfTheCustomerHasOne()
        {
            _sut.RequestRent(_scooterId);

            _sut.CancelOngoingRent(CancellationReason);

            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void CancelOngoingRent_ShouldReturnTheCancelledRent_IfTheCustomerHasOne()
        {
            var rent = _sut.RequestRent(_scooterId).ReadValue();

            _sut.CancelOngoingRent(CancellationReason).ShouldBe(rent);
        }

        [Fact]
        public void CancelOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            _sut.CancelOngoingRent(CancellationReason).ShouldBe(new NoOngoingRent());
        }

        [Fact]
        public void StopOngoingRent_ShouldRemoveTheOngoingRent_IfTheCustomerHasAConfirmedOne()
        {
            _sut.RequestRent(_scooterId);
            _sut.ConfirmOngoingRent(_timestamp1);

            _sut.StopOngoingRent(_timestamp2, StopReason);

            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void StopOngoingRent_ShouldReturnTheStoppedRent_IfTheCustomerHasAConfirmedOne()
        {
            var rent = _sut.RequestRent(_scooterId).ReadValue();
            _sut.ConfirmOngoingRent(_timestamp1);

            _sut.StopOngoingRent(_timestamp2, StopReason).ShouldBe(rent);
        }

        [Fact]
        public void StopOngoingRent_ShouldFail_IfTheOngoingRentIsNotConfirmed()
        {
            _sut.RequestRent(_scooterId);

            _sut.StopOngoingRent(_timestamp1, StopReason).ShouldBe(new RentNotConfirmed());
        }

        [Fact]
        public void StopOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            _sut.StopOngoingRent(_timestamp1, StopReason).ShouldBe(new NoOngoingRent());
        }
    }
}
