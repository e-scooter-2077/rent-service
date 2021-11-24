using EasyDesk.CleanArchitecture.Application.Mediator;
using System;

namespace EScooter.RentService.Application.Queries
{
    public static class GetRent
    {
        public record Query(Guid RentId) : QueryBase<RentSnapshot>;
    }
}
