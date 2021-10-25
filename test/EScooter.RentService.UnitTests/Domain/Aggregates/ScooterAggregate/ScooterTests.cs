﻿using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using Shouldly;
using System;
using Xunit;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;

namespace EScooter.RentService.UnitTests.Domain.Aggregates.ScooterAggregate
{
    public class ScooterTests
    {
        private readonly Scooter _sut;
        private readonly Guid _customerId = Guid.NewGuid();

        public ScooterTests()
        {
            _sut = Scooter.Create(Guid.NewGuid());
        }

        [Fact]
        public void Create_ShouldReturnAScooterInTheDefaultState()
        {
            _sut.ShouldSatisfyAllConditions(
                scooter => scooter.RentingCustomerId.ShouldBeEmpty(),
                scooter => scooter.IsEnabled.ShouldBeTrue(),
                scooter => scooter.IsInStandby.ShouldBeFalse(),
                scooter => scooter.IsOutOfService.ShouldBeFalse());
        }

        [Fact]
        public void Disable_ShouldSetTheScooterAsDisabled()
        {
            _sut.Disable();

            _sut.IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public void Enable_ShouldSetTheScooterAsEnabled()
        {
            _sut.Disable();
            _sut.Enable();

            _sut.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void EnterStandby_ShouldSetTheScooterInStandbyMode()
        {
            _sut.EnterStandby();

            _sut.IsInStandby.ShouldBeTrue();
        }

        [Fact]
        public void LeaveStandby_ShouldSetTheScooterToNotBeInStandbyMode()
        {
            _sut.EnterStandby();
            _sut.LeaveStandby();

            _sut.IsInStandby.ShouldBeFalse();
        }

        [Fact]
        public void LeaveAreaOfService_SholdSetTheScooterAsOutOfService()
        {
            _sut.LeaveAreaOfService();

            _sut.IsOutOfService.ShouldBeTrue();
        }

        [Fact]
        public void EnterAreaOfService_SholdSetTheScooterAsInService()
        {
            _sut.LeaveAreaOfService();
            _sut.EnterAreaOfService();

            _sut.IsOutOfService.ShouldBeFalse();
        }

        [Fact]
        public void Rent_SetTheScooterAsRented_IfTheScooterIsRentableAndAvailable()
        {
            _sut.Rent(_customerId);

            _sut.RentingCustomerId.ShouldBe(_customerId);
        }

        [Fact]
        public void Rent_ShouldReturnOk_IfTheScooterIsRentableAndAvailable()
        {
            _sut.Rent(_customerId).ShouldBe(Ok);
        }

        [Fact]
        public void Rent_ShouldFail_IfTheScooterIsAlreadyRented()
        {
            _sut.Rent(_customerId);

            _sut.Rent(customerId: Guid.NewGuid()).ShouldBe(new AlreadyRented());
        }

        [Fact]
        public void Rent_ShouldFail_IfTheScooterIsDisabled()
        {
            _sut.Disable();

            _sut.Rent(_customerId).ShouldBe(new NotRentable());
        }

        [Fact]
        public void Rent_ShouldFail_IfTheScooterIsOutOfService()
        {
            _sut.LeaveAreaOfService();

            _sut.Rent(_customerId).ShouldBe(new NotRentable());
        }

        [Fact]
        public void Rent_ShouldFail_IfTheScooterIsInStandby()
        {
            _sut.EnterStandby();

            _sut.Rent(_customerId).ShouldBe(new NotRentable());
        }

        [Fact]
        public void ClearRent_ShouldMakeTheScooterAvailable()
        {
            _sut.Rent(_customerId);

            _sut.ClearRent();

            _sut.RentingCustomerId.ShouldBeEmpty();
        }
    }
}