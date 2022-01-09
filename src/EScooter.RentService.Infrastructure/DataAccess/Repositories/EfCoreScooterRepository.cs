using EasyDesk.CleanArchitecture.Dal.EfCore.Repositories;
using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EScooter.RentService.Infrastructure.DataAccess.Repositories;

/// <summary>
/// An implementation for the <see cref="IScooterRepository"/> service using Entity Framework Core to access the data store.
/// </summary>
public class EfCoreScooterRepository : EfCoreRepository<Scooter, ScooterModel, RentDbContext>, IScooterRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreScooterRepository"/> class.
    /// </summary>
    /// <param name="context">The DbContext to use.</param>
    /// <param name="eventNotifier">The domain event notifier.</param>
    public EfCoreScooterRepository(
        RentDbContext context,
        IDomainEventNotifier eventNotifier)
        : base(context, new ScooterConverter(), eventNotifier)
    {
    }

    /// <inheritdoc/>
    protected override DbSet<ScooterModel> GetDbSet(RentDbContext context) => context.Scooters;

    /// <inheritdoc/>
    protected override IQueryable<ScooterModel> Includes(IQueryable<ScooterModel> initialQuery) => initialQuery;

    /// <inheritdoc/>
    public Task<Result<Scooter>> GetById(Guid id) => GetSingle(q => q.Where(s => s.Id == id));
}
