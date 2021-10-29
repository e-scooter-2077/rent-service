using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Domain.Aggregates.PastRentAggregate;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            services.AddScoped<ICustomerRepository>(_ => throw new NotImplementedException());
            services.AddScoped<IScooterRepository>(_ => throw new NotImplementedException());
            services.AddScoped<IPastRentRepository>(_ => throw new NotImplementedException());
        }
    }
}
