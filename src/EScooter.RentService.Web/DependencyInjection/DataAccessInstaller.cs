using EasyDesk.CleanArchitecture.Dal.EfCore;
using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using EScooter.RentService.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EScooter.RentService.Web.DependencyInjection
{
    public class DataAccessInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddEfCoreDataAccess(configuration.GetConnectionString("MainDb"), options =>
            {
                options
                    .AddEntities<RentDbContext>()
                    .AddOutbox()
                    .AddIdemptenceManager();
            });
        }
    }
}
