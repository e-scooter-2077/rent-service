using EasyDesk.CleanArchitecture.Domain.Metamodel.Repositories;
using System;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    public interface ICustomerRepository :
        IGetByIdRepository<Customer, Guid>,
        ISaveRepository<Customer>,
        IRemoveRepository<Customer>
    {
    }
}
