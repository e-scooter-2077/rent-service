using EasyDesk.Testing.MatrixExpansion;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.UnitTests.Infrastructure.Converters;

public class ScooterConverterTests
{
    private readonly ScooterConverter _sut = new();
    private readonly ScooterModel _persistenceModel = new ScooterModel
    {
        Id = Guid.NewGuid()
    };

    [Theory]
    [MemberData(nameof(ScooterStates))]
    public void ToDomain_ShouldCopyBasicProperties(
        bool enabled, bool standby, bool outOfService, Option<Guid> rentId)
    {
        _persistenceModel.IsEnabled = enabled;
        _persistenceModel.IsInStandby = standby;
        _persistenceModel.IsOutOfService = outOfService;
        _persistenceModel.OngoingRentId = rentId.AsNullable();

        _sut.ToDomain(_persistenceModel).ShouldSatisfyAllConditions(
            scooter => scooter.Id.ShouldBe(_persistenceModel.Id),
            scooter => scooter.IsEnabled.ShouldBe(enabled),
            scooter => scooter.IsInStandby.ShouldBe(standby),
            scooter => scooter.IsOutOfService.ShouldBe(outOfService),
            scooter => scooter.OngoingRentId.ShouldBe(rentId));
    }

    [Theory]
    [MemberData(nameof(ScooterStates))]
    public void ApplyChanges_ShouldCopyBasicProperties(
        bool enabled, bool standby, bool outOfService, Option<Guid> rentId)
    {
        var scooter = new Scooter(Guid.NewGuid(), rentId, enabled, outOfService, standby);

        _sut.ApplyChanges(scooter, _persistenceModel);

        _persistenceModel.ShouldSatisfyAllConditions(
            model => model.Id.ShouldBe(scooter.Id),
            model => model.IsEnabled.ShouldBe(enabled),
            model => model.IsInStandby.ShouldBe(standby),
            model => model.IsOutOfService.ShouldBe(outOfService),
            model => model.OngoingRentId.ShouldBe(rentId.AsNullable()));
    }

    public static IEnumerable<object[]> ScooterStates()
    {
        return Matrix
            .Axis(true, false)
            .Axis(true, false)
            .Axis(true, false)
            .Axis(None, Some(Guid.NewGuid()))
            .Build();
    }
}
