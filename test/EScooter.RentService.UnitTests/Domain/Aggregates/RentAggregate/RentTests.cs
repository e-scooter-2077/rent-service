using System;
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
        private readonly Rent _sut;

        public RentTests()
        {
            _sut = Rent.Create(_scooterId, _customerId, RequestTimestamp);
        }

        private Timestamp RequestTimestamp { get; } = Timestamp.Now;

        private RentConfirmationInfo ConfirmationInfo => new(RequestTimestamp + TimeOffset.FromSeconds(1));

        [Fact]
        public void CreateForScooter_ShouldReturnARentInTheInitialState()
        {
            _sut.ShouldSatisfyAllConditions(
                rent => rent.ScooterId.ShouldBe(_scooterId),
                rent => rent.ConfirmationInfo.ShouldBeEmpty(),
                rent => rent.CancellationInfo.ShouldBeEmpty(),
                rent => rent.StopInfo.ShouldBeEmpty(),
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
        public void Confirm_ShouldFail_IfTheRentIsNotInAPendingState()
        {
            _sut.Confirm(ConfirmationInfo);

            var newConfirmation = ConfirmationInfo with { Timestamp = ConfirmationInfo.Timestamp + TimeOffset.FromSeconds(1) };
            _sut.Confirm(newConfirmation).ShouldBe(new InvalidRentState(RentState.Ongoing));
        }
    }
}
