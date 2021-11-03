using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using Shouldly;
using System;
using Xunit;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.CustomerAggregate
{
    public class CustomerTests
    {
        private readonly Customer _sut;
        private readonly Guid _rentId = Guid.NewGuid();

        public CustomerTests()
        {
            _sut = Customer.Create(Guid.NewGuid());
        }

        [Fact]
        public void Create_ShouldCreateACustomerWithNoActiveRent()
        {
            _sut.OngoingRentId.ShouldBeEmpty();
        }

        [Fact]
        public void StartRent_ShouldSucceed_IfTheCustomerHasNoOngoingRent()
        {
            _sut.StartRent(_rentId).ShouldBe(Ok);
        }

        [Fact]
        public void StartRent_ShouldStartANewRent_IfTheCustomerHasNoOngoingRent()
        {
            _sut.StartRent(_rentId);

            _sut.OngoingRentId.ShouldContain(_rentId);
        }

        [Fact]
        public void StartRent_ShouldFail_IfTheCustomerAlreadyHasAnOngoingRent()
        {
            _sut.StartRent(_rentId);

            _sut.StartRent(Guid.NewGuid()).ShouldBe(new RentAlreadyOngoing());
        }

        [Fact]
        public void EndRent_ShouldSucceed_IfTheCustomerHasAnOngoingRent()
        {
            _sut.StartRent(_rentId);

            _sut.EndRent().ShouldBe(Ok);
        }

        [Fact]
        public void EndRent_ShouldRemoveTheOngoingRent_IfTheCustomerHasAnOngoingRent()
        {
            _sut.StartRent(_rentId);
            _sut.EndRent();

            _sut.OngoingRentId.ShouldBeEmpty();
        }

        [Fact]
        public void EndRent_ShouldFail_IfTheCustomerHasNoOngoingRent()
        {
            _sut.EndRent().ShouldBe(new NoOngoingRent());
        }
    }
}
