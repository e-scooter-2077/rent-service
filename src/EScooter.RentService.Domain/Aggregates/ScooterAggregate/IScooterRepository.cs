using EasyDesk.CleanArchitecture.Domain.Metamodel.Repositories;
using System;

namespace EScooter.RentService.Domain.Aggregates.ScooterAggregate;

public interface IScooterRepository :
    IGetByIdRepository<Scooter, Guid>,
    ISaveRepository<Scooter>,
    IRemoveRepository<Scooter>
{
}
