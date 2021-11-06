using EasyDesk.CleanArchitecture.Web.Controllers;
using EScooter.RentService.Application.Commands.Rents;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Web.Controllers.V_1_0
{
    public record RequestRentBodyDto(Guid CustomerId, Guid ScooterId);

    public class RentsController : AbstractMediatrController
    {
        [HttpPost("rents")]
        public async Task<IActionResult> RequestRent([FromBody] RequestRentBodyDto body)
        {
            var command = new RequestRent.Command(body.CustomerId, body.ScooterId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }

        [HttpPut("rents/{rentId}/stop")]
        public async Task<IActionResult> RequestRent([FromRoute] Guid rentId)
        {
            var command = new StopRent.Command(rentId);
            return await Command(command)
                .ReturnOk()
                .MapEmpty();
        }
    }
}
