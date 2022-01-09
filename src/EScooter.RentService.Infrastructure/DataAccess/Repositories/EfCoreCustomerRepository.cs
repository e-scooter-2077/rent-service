using EasyDesk.CleanArchitecture.Dal.EfCore.Repositories;
using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Converters;
using EScooter.RentService.Infrastructure.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EScooter.RentService.Infrastructure.DataAccess.Repositories;

/// <summary>
/// An implementation for the <see cref="ICustomerRepository"/> service using Entity Framework Core to access the data store.
/// </summary>
public class EfCoreCustomerRepository : EfCoreRepository<Customer, CustomerModel, RentDbContext>, ICustomerRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreCustomerRepository"/> class.
    /// </summary>
    /// <param name="context">The DbContext to use.</param>
    /// <param name="eventNotifier">The domain event notifier.</param>
    public EfCoreCustomerRepository(
        RentDbContext context,
        IDomainEventNotifier eventNotifier)
        : base(context, new CustomerConverter(), eventNotifier)
    {
    }

    /// <inheritdoc/>
    protected override DbSet<CustomerModel> GetDbSet(RentDbContext context) => context.Customers;

    /// <inheritdoc/>
    protected override IQueryable<CustomerModel> Includes(IQueryable<CustomerModel> initialQuery) => initialQuery;

    /// <inheritdoc/>
    public Task<Result<Customer>> GetById(Guid id) => GetSingle(q => q.Where(c => c.Id == id));
}
