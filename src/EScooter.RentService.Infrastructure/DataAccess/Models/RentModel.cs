using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using EScooter.RentService.Domain.Aggregates.RentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EScooter.RentService.Infrastructure.DataAccess.Models;

public class RentModel
{
    public Guid Id { get; set; }

    public Guid ScooterId { get; set; }

    public Guid CustomerId { get; set; }

    public Timestamp RequestTimestamp { get; set; }

    public Timestamp ConfirmationTimestamp { get; set; }

    public Timestamp StopTimestamp { get; set; }

    public RentStopReason? StopReason { get; set; }

    public RentCancellationReason? CancellationReason { get; set; }

    public CustomerModel Customer { get; set; }

    public ScooterModel Scooter { get; set; }

    public class Configuration : IEntityTypeConfiguration<RentModel>
    {
        public void Configure(EntityTypeBuilder<RentModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestTimestamp)
                .IsRequired();

            builder.Property(x => x.StopReason)
                .HasConversion(new EnumToStringConverter<RentStopReason>());

            builder.Property(x => x.CancellationReason)
                .HasConversion(new EnumToStringConverter<RentCancellationReason>());

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Rents)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Scooter)
                .WithMany(x => x.Rents)
                .HasForeignKey(x => x.ScooterId);
        }
    }
}
