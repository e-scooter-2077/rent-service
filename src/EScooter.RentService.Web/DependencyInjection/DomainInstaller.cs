using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Repositories;
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
            services.AddScoped<ICustomerRepository, EfCoreCustomerRepository>();
            services.AddScoped<IScooterRepository, EfCoreScooterRepository>();
            services.AddScoped<IRentRepository, EfCoreRentRepository>();
        }
    }
}
