using EasyDesk.CleanArchitecture.Dal.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace EScooter.RentService.Infrastructure.DataAccess
{
    /// <summary>
    /// Represents the Entity Framework Core <see cref="DbContext"/> for the Rent service.
    /// </summary>
    public class RentDbContext : EntitiesContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentDbContext"/> class.
        /// </summary>
        /// <param name="options">The DbContext options.</param>
        public RentDbContext(DbContextOptions<RentDbContext> options) : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
