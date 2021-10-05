using System;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using Shouldly;
using Xunit;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.CustomerAggregate
{
    public class RentTests
    {
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
    }
}
