using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ___MICROSERVICE_NAME___.Web.DependencyInjection
{
    /// <summary>
    /// A service installer that configures swagger for the application.
    /// </summary>
    public class SwaggerInstaller : IServiceInstaller
    {
        /// <inheritdoc/>
        public void InstallServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddSwagger("___MICROSERVICE_NAME___", typeof(Startup));
        }
    }
}
