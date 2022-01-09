using EasyDesk.CleanArchitecture.Domain.Metamodel.Repositories;
using System;

namespace EScooter.RentService.Domain.Aggregates.RentAggregate;

public interface IRentRepository :
    IGetByIdRepository<Rent, Guid>,
    ISaveRepository<Rent>
{
}
