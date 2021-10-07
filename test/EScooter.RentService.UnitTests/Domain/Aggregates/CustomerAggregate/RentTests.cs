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

            _sut.Confirm(_timestamp2).ShouldBe(new RentAlreadyConfirmed());
        }
    }
}
