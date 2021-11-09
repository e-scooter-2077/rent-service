using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.Tools.Options;
using System;

namespace EScooter.RentService.Application.Queries
{
    public static class GetScooters
    {
        public record Query(
            Option<bool> Rentable,
            Option<bool> Available,
            Pagination Pagination) : PaginatedQueryBase<ScooterSnapshot>(Pagination);

        public record ScooterSnapshot(
            Guid Id,
            bool Rentable,
            Option<Guid> OngoingRentId);
    }
}
