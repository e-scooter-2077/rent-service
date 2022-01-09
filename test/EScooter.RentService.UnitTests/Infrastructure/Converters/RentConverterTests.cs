using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Shouldly;
using System;
using Xunit;

namespace EScooter.RentService.UnitTests.Infrastructure.Converters;

public class RentConverterTests
{
    private readonly RentConverter _sut = new();
    private readonly RentModel _persistenceModel = new RentModel
    {
        Id = Guid.NewGuid(),
        ScooterId = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        RequestTimestamp = Timestamp.Now
    };

    private readonly Rent _domainModel = Rent.Create(Guid.NewGuid(), Guid.NewGuid(), Timestamp.Now);

    [Fact]
    public void ToDomain_ShouldCopyBasicProperties()
    {
        _sut.ToDomain(_persistenceModel).ShouldSatisfyAllConditions(
            rent => rent.Id.ShouldBe(_persistenceModel.Id),
            rent => rent.ScooterId.ShouldBe(_persistenceModel.ScooterId),
            rent => rent.CustomerId.ShouldBe(_persistenceModel.CustomerId),
            rent => rent.RequestTimestamp.ShouldBe(_persistenceModel.RequestTimestamp),
            rent => rent.ConfirmationInfo.ShouldBeEmpty(),
            rent => rent.CancellationInfo.ShouldBeEmpty(),
            rent => rent.StopInfo.ShouldBeEmpty());
    }

    [Fact]
    public void ToDomain_ShouldCopyConfirmationInfo_IfPresent()
    {
        var confirmationTimestamp = Timestamp.Now;
        _persistenceModel.ConfirmationTimestamp = confirmationTimestamp;

        _sut.ToDomain(_persistenceModel).ConfirmationInfo.ShouldContain(new RentConfirmationInfo(confirmationTimestamp));
    }

    [Fact]
    public void ToDomain_ShouldCopyCancellationInfo_IfPresent()
    {
        var cancellationReason = RentCancellationReason.CreditInsufficient;
        _persistenceModel.CancellationReason = cancellationReason;

        _sut.ToDomain(_persistenceModel).CancellationInfo.ShouldContain(new RentCancellationInfo(cancellationReason));
    }

    [Fact]
    public void ToDomain_ShouldCopyStopInfo_IfPresent()
    {
        var stopReason = RentStopReason.StoppedByCustomer;
        var confirmationTimestamp = Timestamp.Now;
        var stopTimestamp = confirmationTimestamp + TimeOffset.FromMinutes(10);
        _persistenceModel.StopReason = stopReason;
        _persistenceModel.ConfirmationTimestamp = confirmationTimestamp;
        _persistenceModel.StopTimestamp = stopTimestamp;

        _sut.ToDomain(_persistenceModel).StopInfo.ShouldContain(new RentStopInfo(stopReason, stopTimestamp));
    }

    [Fact]
    public void ApplyChanges_ShouldCopyBasicProperties()
    {
        _sut.ApplyChanges(_domainModel, _persistenceModel);

        _persistenceModel.ShouldSatisfyAllConditions(
            model => model.Id.ShouldBe(_domainModel.Id),
            model => model.ScooterId.ShouldBe(_domainModel.ScooterId),
            model => model.CustomerId.ShouldBe(_domainModel.CustomerId),
            model => model.RequestTimestamp.ShouldBe(_domainModel.RequestTimestamp),
            model => model.ConfirmationTimestamp.ShouldBeNull(),
            model => model.CancellationReason.ShouldBeNull(),
            model => model.StopReason.ShouldBeNull(),
            model => model.StopTimestamp.ShouldBeNull());
    }

    [Fact]
    public void ApplyChanges_ShouldCopyConfirmationInfo_IfPresent()
    {
        var confirmationTimestamp = Timestamp.Now;
        _domainModel.Confirm(new(confirmationTimestamp));

        _sut.ApplyChanges(_domainModel, _persistenceModel);

        _persistenceModel.ConfirmationTimestamp.ShouldBe(confirmationTimestamp);
    }

    [Fact]
    public void ApplyChanges_ShouldCopyCancellationInfo_IfPresent()
    {
        var cancellationReason = RentCancellationReason.CreditInsufficient;
        _domainModel.Cancel(new(cancellationReason));

        _sut.ApplyChanges(_domainModel, _persistenceModel);

        _persistenceModel.CancellationReason.ShouldBe(cancellationReason);
    }

    [Fact]
    public void ApplyChanges_ShouldCopyStopInfo_IfPresent()
    {
        var confirmationTimestamp = Timestamp.Now;
        var stopTimestamp = confirmationTimestamp + TimeOffset.FromMinutes(10);
        var stopReason = RentStopReason.StoppedByCustomer;
        _domainModel.Confirm(new(confirmationTimestamp));
        _domainModel.Stop(new(stopReason, stopTimestamp));

        _sut.ApplyChanges(_domainModel, _persistenceModel);

        _persistenceModel.ShouldSatisfyAllConditions(
            rent => rent.StopReason.ShouldBe(stopReason),
            rent => rent.StopTimestamp.ShouldBe(stopTimestamp));
    }
}
