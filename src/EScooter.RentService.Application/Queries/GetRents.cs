using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.Tools.Options;
using System;

namespace EScooter.RentService.Application.Queries;

public static partial class GetRents
{
    public record Query(
        Option<Guid> CustomerId,
        Option<Guid> ScooterId,
        Pagination Pagination) : PaginatedQueryBase<RentSnapshot>(Pagination);
}
