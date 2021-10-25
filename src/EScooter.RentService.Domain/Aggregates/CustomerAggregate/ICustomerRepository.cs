using EasyDesk.CleanArchitecture.Domain.Metamodel.Repositories;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using System;
using System.Threading.Tasks;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    public interface ICustomerRepository :
        IGetByIdRepository<Customer, Guid>,
        ISaveRepository<Customer>,
        IRemoveRepository<Customer>
    {
        Task<Result<Customer>> GetByRent(Guid rentId);
    }
}
