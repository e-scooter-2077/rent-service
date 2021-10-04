using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EScooter.RentService.Web.DependencyInjection
{
    /// <summary>
    /// An installer containing dependency injection configuration for the domain layer.
    /// </summary>
    public class DomainInstaller : IServiceInstaller
    {
        /// <inheritdoc/>
        public void InstallServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            // TODO: Register domain services here.
        }
    }
}
