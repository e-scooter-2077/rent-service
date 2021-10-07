using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.Tools.Options;
using System;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    public class Customer : AggregateRoot<Customer>
    {
        public Customer(Guid id, Option<Rent> currentRent) : base(id)
        {
            CurrentRent = currentRent;
        }

        public Option<Rent> CurrentRent { get; private set; }
    }
}
