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

        private RentConfirmationInfo ConfirmationInfo => new(RequestTimestamp + TimeOffset.FromSeconds(1));

        [Fact]
        public void CreateForScooter_ShouldReturnARentInTheInitialState()
        {
            _sut.ShouldSatisfyAllConditions(
                rent => rent.ScooterId.ShouldBe(_scooterId),
                rent => rent.ConfirmationInfo.ShouldBeEmpty(),
                rent => rent.RequestTimestamp.ShouldBe(RequestTimestamp));
        }

        [Fact]
        public void Confirm_ShouldFillTheRentWithConfirmationInfo_IfTheRentIsPending()
        {
            _sut.Confirm(ConfirmationInfo);

            _sut.ConfirmationInfo.ShouldContain(ConfirmationInfo);
        }

        [Fact]
        public void Confirm_ShouldReturnOk_IfTheRentIsPending()
        {
            _sut.Confirm(ConfirmationInfo).ShouldBe(Ok);
        }

        [Fact]
        public void Confirm_ShouldFail_IfTheRentHasAlreadyBeenConfirmed()
        {
            _sut.Confirm(ConfirmationInfo);

            var newConfirmation = ConfirmationInfo with { Timestamp = ConfirmationInfo.Timestamp + TimeOffset.FromSeconds(1) };
            _sut.Confirm(newConfirmation).ShouldBe(new RentAlreadyConfirmed());
        }
    }
}
