using EasyDesk.CleanArchitecture.Dal.EfCore.ModelConversion;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Models;

namespace EScooter.RentService.Infrastructure.DataAccess.Converters
{
    public class CustomerConverter : IModelConverter<Customer, CustomerModel>
    {
        public Customer ToDomain(CustomerModel model)
        {
            return new Customer(model.Id, model.OngoingRentId.AsOption());
        }

        public void ApplyChanges(Customer origin, CustomerModel destination)
        {
            destination.Id = origin.Id;
            destination.OngoingRentId = origin.OngoingRentId.AsNullable();
        }
    }
}
