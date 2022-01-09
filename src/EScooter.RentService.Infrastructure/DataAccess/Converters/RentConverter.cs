using EasyDesk.CleanArchitecture.Dal.EfCore.ModelConversion;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Models;

namespace EScooter.RentService.Infrastructure.DataAccess.Converters;

public class RentConverter : IModelConverter<Rent, RentModel>
{
    public Rent ToDomain(RentModel model)
    {
        var confirmationInfo = model.ConfirmationTimestamp
            .AsOption()
            .Map(t => new RentConfirmationInfo(t));
        var cancellationInfo = model.CancellationReason
            .AsOption()
            .Map(r => new RentCancellationInfo(r));
        var stopInfo = model.StopReason
            .AsOption()
            .Map(r => new RentStopInfo(r, model.StopTimestamp));
        return new Rent(
            model.Id,
            model.ScooterId,
            model.CustomerId,
            model.RequestTimestamp,
            confirmationInfo,
            cancellationInfo,
            stopInfo);
    }

    public void ApplyChanges(Rent origin, RentModel destination)
    {
        destination.Id = origin.Id;
        destination.ScooterId = origin.ScooterId;
        destination.CustomerId = origin.CustomerId;
        destination.RequestTimestamp = origin.RequestTimestamp;
        destination.ConfirmationTimestamp = origin.ConfirmationInfo.Map(c => c.Timestamp).OrElseNull();
        destination.CancellationReason = origin.CancellationInfo.Map(c => c.Reason).AsNullable();
        var (stopReason, stopTimestamp) = origin.StopInfo.Match<(RentStopReason?, Timestamp)>(
            some: info => (info.Reason, info.Timestamp),
            none: () => (null, null));
        destination.StopReason = stopReason;
        destination.StopTimestamp = stopTimestamp;
    }
}
