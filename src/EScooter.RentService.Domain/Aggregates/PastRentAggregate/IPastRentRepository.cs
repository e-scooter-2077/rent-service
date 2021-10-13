using EasyDesk.CleanArchitecture.Domain.Metamodel.Repositories;

namespace EScooter.RentService.Domain.Aggregates.PastRentAggregate
{
    public interface IPastRentRepository :
        ISaveRepository<PastRent>
    {
    }
}
