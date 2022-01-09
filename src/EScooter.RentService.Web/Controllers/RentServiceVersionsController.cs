using EasyDesk.CleanArchitecture.Web.Controllers;

namespace EScooter.RentService.Web.Controllers;

public class RentServiceVersionsController : VersionsController
{
    public RentServiceVersionsController() : base(typeof(Startup))
    {
    }
}
