using EasyDesk.CleanArchitecture.Dal.EfCore.ModelConversion;
using EasyDesk.Tools.Options;
using EScooter.RentService.Domain.Aggregates.ScooterAggregate;
using EScooter.RentService.Infrastructure.DataAccess.Models;

namespace EScooter.RentService.Infrastructure.DataAccess.Converters
{
    public class ScooterConverter : IModelConverter<Scooter, ScooterModel>
    {
        public Scooter ToDomain(ScooterModel model)
        {
            return new Scooter(
                model.Id,
                model.OngoingRentId.AsOption(),
                model.IsEnabled,
                model.IsOutOfService,
                model.IsInStandby);
        }

        public void ApplyChanges(Scooter origin, ScooterModel destination)
        {
            destination.Id = origin.Id;
            destination.OngoingRentId = origin.OngoingRentId.AsNullable();
            destination.IsEnabled = origin.IsEnabled;
            destination.IsOutOfService = origin.IsOutOfService;
            destination.IsInStandby = origin.IsInStandby;
        }
    }
}
