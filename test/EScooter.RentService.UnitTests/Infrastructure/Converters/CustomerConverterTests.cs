using EasyDesk.Testing.MatrixExpansion;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.UnitTests.Infrastructure.Converters
{
    public class CustomerConverterTests
    {
        private readonly CustomerConverter _sut = new();
        private readonly CustomerModel _persistenceModel = new CustomerModel
        {
            Id = Guid.NewGuid()
        };

        [Theory]
        [MemberData(nameof(CustomerStates))]
        public void ToDomain_ShouldCopyBasicProperties(Option<Guid> rentId)
        {
            _persistenceModel.OngoingRentId = rentId.AsNullable();

            _sut.ToDomain(_persistenceModel).ShouldSatisfyAllConditions(
                customer => customer.Id.ShouldBe(_persistenceModel.Id),
                customer => customer.OngoingRentId.ShouldBe(rentId));
        }

        [Theory]
        [MemberData(nameof(CustomerStates))]
        public void ApplyChanges_ShouldCopyBasicProperties(Option<Guid> rentId)
        {
            var customer = new Customer(Guid.NewGuid(), rentId);

            _sut.ApplyChanges(customer, _persistenceModel);

            _persistenceModel.ShouldSatisfyAllConditions(
                model => model.Id.ShouldBe(customer.Id),
                model => model.OngoingRentId.ShouldBe(rentId.AsNullable()));
        }

        public static IEnumerable<object[]> CustomerStates()
        {
            return Matrix
                .Axis(None, Some(Guid.NewGuid()))
                .Build();
        }
    }
}
