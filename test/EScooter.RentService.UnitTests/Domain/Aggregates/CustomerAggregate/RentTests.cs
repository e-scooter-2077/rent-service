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
        private readonly Guid _scooterId = Guid.NewGuid();
        private readonly Rent _sut;

        public RentTests()
        {
            _sut = Rent.CreateForScooter(_scooterId, RequestTimestamp);
        }

        private Timestamp RequestTimestamp { get; } = Timestamp.Now;

        private Timestamp ConfirmationTimestamp => RequestTimestamp + TimeOffset.FromSeconds(1);

        [Fact]
        public void CreateForScooter_ShouldReturnARentInTheInitialState()
        {
            _sut.ScooterId.ShouldBe(_scooterId);
            _sut.ConfirmationInfo.ShouldBeEmpty();
            _sut.RequestTimestamp.ShouldBe(RequestTimestamp);
        }

        [Fact]
        public void Confirm_ShouldFillTheRentWithConfirmationInfo_IfTheRentIsPending()
        {
            _sut.Confirm(ConfirmationTimestamp);

            _sut.ConfirmationInfo.ShouldContain(new RentConfirmationInfo(ConfirmationTimestamp));
        }

        [Fact]
        public void Confirm_ShouldReturnOk_IfTheRentIsPending()
        {
            _sut.Confirm(ConfirmationTimestamp).ShouldBe(Ok);
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(ConfirmationTimestamp);

            _sut.Confirm(ConfirmationTimestamp + TimeOffset.FromSeconds(1)).ShouldBe(new RentAlreadyConfirmed());
        }
    }
}
