using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.CleanArchitecture.Testing.Domain;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using Shouldly;
using System;
using Xunit;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.CustomerAggregate
{
    public class CustomerTests
    {
        private readonly Customer _sut;
        private readonly Guid _scooterId = Guid.NewGuid();
        private readonly Timestamp _requestTimestamp = Timestamp.Now;

        public CustomerTests()
        {
            _sut = Customer.Create(Guid.NewGuid());
        }

        private RentConfirmationInfo ConfirmationInfo => new(_requestTimestamp + TimeOffset.FromSeconds(1));

        private RentCancellationInfo CancellationInfo => new(RentCancellationReason.InternalError);

        private RentStopInfo StopInfo => new(RentStopReason.StoppedByCustomer, ConfirmationInfo.Timestamp + TimeOffset.FromMinutes(20));

        private Rent RequestRent() => _sut.RequestRent(_scooterId, _requestTimestamp).ReadValue();

        private Result<Rent> ConfirmRent() => _sut.ConfirmOngoingRent(ConfirmationInfo);

        private Result<Rent> CancelRent() => _sut.CancelOngoingRent(CancellationInfo);

        private Result<Rent> StopRent() => _sut.StopOngoingRent(StopInfo);

        [Fact]
        public void Create_ShouldCreateACustomerWithNoActiveRent()
        {
            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void RequestRent_ShouldReturnTheCreatedRent_IfTheCustomerHasNoOngoingRent()
        {
            RequestRent().ShouldSatisfyAllConditions(
                rent => rent.ScooterId.ShouldBe(_scooterId),
                rent => rent.ConfirmationInfo.ShouldBeEmpty());
        }

        [Fact]
        public void RequestRent_ShouldStartANewRent_IfTheCustomerHasNoOngoingRent()
        {
            var rent = RequestRent();

            _sut.OngoingRent.ShouldContain(rent);
        }

        [Fact]
        public void RequestRent_ShouldEmitAnEvent_IfTheRequestIsSuccessful()
        {
            var rent = RequestRent();

            _sut.ShouldHaveEmitted(new RentRequestedEvent(_sut, rent));
        }

        [Fact]
        public void RequestRent_ShouldFail_IfTheCustomerAlreadyHasAnOngoingRent()
        {
            RequestRent();

            _sut.RequestRent(Guid.NewGuid(), _requestTimestamp).ShouldBe(new RentAlreadyOngoing());
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldReturnTheOngoingRent_IfTheCustomerHasOne()
        {
            var rent = RequestRent();

            ConfirmRent().ShouldBe(rent);
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldConfirmTheOngoingRent_IfTheCustomerHasOne()
        {
            RequestRent();

            ConfirmRent();

            _sut.OngoingRent.Value.ConfirmationInfo.ShouldContain(ConfirmationInfo);
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldEmitAnEvent_IfTheConfirmationWasSuccessful()
        {
            var rent = RequestRent();

            ConfirmRent();

            _sut.ShouldHaveEmitted(new RentConfirmedEvent(_sut, rent, ConfirmationInfo));
        }

        [Fact]
        public void ConfirmOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            ConfirmRent().ShouldBe(new NoOngoingRent());
        }

        [Fact]
        public void CancelOngoingRent_ShouldRemoveTheOngoingRent_IfTheCustomerHasOne()
        {
            RequestRent();

            CancelRent();

            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void CancelOngoingRent_ShouldReturnTheCancelledRent_IfTheCustomerHasOne()
        {
            var rent = RequestRent();

            CancelRent().ShouldBe(rent);
        }

        [Fact]
        public void CancelOngoingRent_ShouldEmitAnEvent_IfTheCancellationIsSuccessful()
        {
            var rent = RequestRent();

            CancelRent();

            _sut.ShouldHaveEmitted(new RentCancelledEvent(_sut, rent, CancellationInfo));
        }

        [Fact]
        public void CancelOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            CancelRent().ShouldBe(new NoOngoingRent());
        }

        [Fact]
        public void StopOngoingRent_ShouldRemoveTheOngoingRent_IfTheCustomerHasAConfirmedOne()
        {
            RequestRent();
            ConfirmRent();

            StopRent();

            _sut.OngoingRent.ShouldBeEmpty();
        }

        [Fact]
        public void StopOngoingRent_ShouldReturnTheStoppedRent_IfTheCustomerHasAConfirmedOne()
        {
            var rent = RequestRent();
            ConfirmRent();

            StopRent().ShouldBe(rent);
        }

        [Fact]
        public void StopOngoingRent_ShouldEmitAnEvent_IfTheStopIsSuccessful()
        {
            var rent = RequestRent();
            ConfirmRent();

            StopRent();

            _sut.ShouldHaveEmitted(new RentStoppedEvent(_sut, rent, StopInfo));
        }

        [Fact]
        public void StopOngoingRent_ShouldFail_IfTheOngoingRentIsNotConfirmed()
        {
            RequestRent();

            StopRent().ShouldBe(new RentNotConfirmed());
        }

        [Fact]
        public void StopOngoingRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            StopRent().ShouldBe(new NoOngoingRent());
        }
    }
}
