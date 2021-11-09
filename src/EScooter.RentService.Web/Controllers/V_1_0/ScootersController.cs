using EasyDesk.CleanArchitecture.Application.Mapping;
using EasyDesk.CleanArchitecture.Application.Pages;
using EasyDesk.CleanArchitecture.Web.Controllers;
using EasyDesk.CleanArchitecture.Web.Dto;
using EasyDesk.Tools.Options;
using EScooter.RentService.Application.Commands.Scooters;
using EScooter.RentService.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Web.Controllers.V_1_0
{
    public record ScooterDto(
            Guid Id,
            bool Rentable,
            Guid? OngoingRentId)
    {
        public class MappingFromScooterSnapshot : DirectMapping<GetScooters.ScooterSnapshot, ScooterDto>
        {
            public MappingFromScooterSnapshot() : base(src => new(
                src.Id,
                src.Rentable,
                src.OngoingRentId.AsNullable()))
            {
            }
        }
    }

    public class ScootersController : AbstractMediatrController
    {
        [HttpGet("scooters")]
        public async Task<IActionResult> GetScooters(
            [FromQuery] bool? available,
            [FromQuery] bool? rentable,
            [FromQuery] PaginationDto pagination)
        {
            var query = new GetScooters.Query(
                rentable.AsOption(),
                available.AsOption(),
                Mapper.Map<Pagination>(pagination));
            return await Query(query)
                .ReturnOk()
                .MapToMany<GetScooters.ScooterSnapshot, ScooterDto>();
        }

        [HttpPost("scooters/{scooterId}/enable")]
        public async Task<IActionResult> EnableScooter([FromRoute] Guid scooterId)
        {
            var command = new EnableScooter.Command(scooterId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }

        [HttpPost("scooters/{scooterId}/disable")]
        public async Task<IActionResult> DisableScooter([FromRoute] Guid scooterId)
        {
            var command = new DisableScooter.Command(scooterId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }
    }
}
