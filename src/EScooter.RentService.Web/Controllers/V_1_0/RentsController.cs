using EasyDesk.CleanArchitecture.Application.Mapping;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.CleanArchitecture.Web.Controllers;
using EasyDesk.CleanArchitecture.Web.Dto;
using EasyDesk.Tools.Options;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Application.Commands.Rents;
using EScooter.RentService.Application.Queries;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Web.Controllers.V_1_0
{
    public record RequestRentBodyDto(Guid CustomerId, Guid ScooterId);

    public record RentDto(
        Guid Id,
        Guid CustomerId,
        Guid ScooterId,
        Timestamp RequestTimestamp,
        RentConfirmationInfo ConfirmationInfo,
        RentCancellationInfo CancellationInfo,
        RentStopInfo StopInfo)
    {
        public class MappingFromRentSnapshot : DirectMapping<GetRents.RentSnapshot, RentDto>
        {
            public MappingFromRentSnapshot() : base(src => new(
                src.Id,
                src.CustomerId,
                src.ScooterId,
                src.RequestTimestamp,
                src.ConfirmationInfo.OrElseNull(),
                src.CancellationInfo.OrElseNull(),
                src.StopInfo.OrElseNull()))
            {
            }
        }
    }

    public class RentsController : AbstractMediatrController
    {
        [HttpGet("rents")]
        public async Task<IActionResult> GetRents(
            [FromQuery] Guid? customerId,
            [FromQuery] Guid? scooterId,
            [FromQuery] PaginationDto pagination)
        {
            var query = new GetRents.Query(
                customerId.AsOption(),
                scooterId.AsOption(),
                Mapper.Map<Pagination>(pagination));
            return await Query(query)
                .ReturnOk()
                .MapToMany<GetRents.RentSnapshot, RentDto>();
        }

        [HttpPost("rents")]
        public async Task<IActionResult> RequestRent([FromBody] RequestRentBodyDto body)
        {
            var command = new RequestRent.Command(body.CustomerId, body.ScooterId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }

        [HttpPost("rents/{rentId}/stop")]
        public async Task<IActionResult> StopRent([FromRoute] Guid rentId)
        {
            var command = new StopRent.Command(rentId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }
    }
}
