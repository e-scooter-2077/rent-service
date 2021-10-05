using EasyDesk.Tools.PrimitiveTypes.DateAndTime;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    public record RentEndInfo(RentEndReason Reason, Timestamp Timestamp);
}
