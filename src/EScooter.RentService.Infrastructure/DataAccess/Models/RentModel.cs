using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EScooter.RentService.Infrastructure.DataAccess.Models
{
    public class RentModel
    {
        public Guid Id { get; set; }

        public Guid ScooterId { get; set; }

        public Guid CustomerId { get; set; }

        public Timestamp RequestTimestamp { get; set; }

        public Timestamp ConfirmationTimestamp { get; set; }

        public Timestamp StopTimestamp { get; set; }

        public string StopReason { get; set; }

        public string CancellationReason { get; set; }

        public CustomerModel Customer { get; set; }

        public ScooterModel Scooter { get; set; }

        public class Configuration : IEntityTypeConfiguration<RentModel>
        {
            public void Configure(EntityTypeBuilder<RentModel> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.RequestTimestamp)
                    .IsRequired();

                builder.HasOne(x => x.Customer)
                    .WithMany(x => x.Rents)
                    .HasForeignKey(x => x.CustomerId);

                builder.HasOne(x => x.Scooter)
                    .WithMany(x => x.Rents)
                    .HasForeignKey(x => x.ScooterId);
            }
        }
    }
}
