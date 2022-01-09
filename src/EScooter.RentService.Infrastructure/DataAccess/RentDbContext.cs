using EasyDesk.CleanArchitecture.Dal.EfCore.Entities;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EScooter.RentService.Infrastructure.DataAccess;

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

    /// <summary>
    /// The table containing rents.
    /// </summary>
    public DbSet<RentModel> Rents { get; set; }

    /// <summary>
    /// The table containing customers.
    /// </summary>
    public DbSet<CustomerModel> Customers { get; set; }

    /// <summary>
    /// The table containing scooters.
    /// </summary>
    public DbSet<ScooterModel> Scooters { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
