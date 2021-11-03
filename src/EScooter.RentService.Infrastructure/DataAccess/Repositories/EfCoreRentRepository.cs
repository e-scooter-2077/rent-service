using EasyDesk.CleanArchitecture.Dal.EfCore.Repositories;
using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EScooter.RentService.Infrastructure.DataAccess.Repositories
{
    /// <summary>
    /// An implementation for the <see cref="IRentRepository"/> service using Entity Framework Core to access the data store.
    /// </summary>
    public class EfCoreRentRepository : EfCoreRepository<Rent, RentModel, RentDbContext>, IRentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRentRepository"/> class.
        /// </summary>
        /// <param name="context">The DbContext to use.</param>
        /// <param name="eventNotifier">The domain event notifier.</param>
        public EfCoreRentRepository(
            RentDbContext context,
            IDomainEventNotifier eventNotifier)
            : base(context, new RentConverter(), eventNotifier)
        {
        }

        /// <inheritdoc/>
        protected override DbSet<RentModel> GetDbSet(RentDbContext context) => context.Rents;

        /// <inheritdoc/>
        protected override IQueryable<RentModel> Includes(IQueryable<RentModel> initialQuery) => initialQuery;

        /// <inheritdoc/>
        public Task<Result<Rent>> GetById(Guid id) => GetSingle(q => q.Where(r => r.Id == id));
    }
}
