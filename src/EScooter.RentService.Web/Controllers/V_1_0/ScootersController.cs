using EasyDesk.CleanArchitecture.Web.Controllers;
using EScooter.RentService.Application.Commands.Scooters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Web.Controllers.V_1_0
{
    public class ScootersController : AbstractMediatrController
    {
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
