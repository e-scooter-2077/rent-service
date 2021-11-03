using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace EScooter.RentService.Infrastructure.DataAccess.Models
{
    public class ScooterModel
    {
        public ScooterModel()
        {
            Rents = new HashSet<RentModel>();
        }

        public Guid Id { get; set; }

        public Guid? OngoingRentId { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsOutOfService { get; set; }

        public bool IsInStandby { get; set; }

        public RentModel OngoingRent { get; set; }

        public ICollection<RentModel> Rents { get; set; }

        public class Configuration : IEntityTypeConfiguration<ScooterModel>
        {
            public void Configure(EntityTypeBuilder<ScooterModel> builder)
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.OngoingRent)
                    .WithOne()
                    .HasForeignKey<ScooterModel>(x => x.OngoingRentId);
            }
        }
    }
}
