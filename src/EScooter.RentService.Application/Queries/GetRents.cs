using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using System;

namespace EScooter.RentService.Application.Queries
{
    public static class GetRents
    {
        public record Query(
            Option<Guid> CustomerId,
            Option<Guid> ScooterId,
            Pagination Pagination) : PaginatedQueryBase<RentSnapshot>(Pagination);

        public record RentSnapshot(
            Guid Id,
            Guid CustomerId,
            Guid ScooterId,
            Timestamp RequestTimestamp,
            Option<RentConfirmationInfo> ConfirmationInfo,
            Option<RentCancellationInfo> CancellationInfo,
            Option<RentStopInfo> StopInfo);
    }
}
