using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace EScooter.RentService.Infrastructure.DataAccess.Models;

public class CustomerModel
{
    public CustomerModel()
    {
        Rents = new HashSet<RentModel>();
    }

    public Guid Id { get; set; }

    public Guid? OngoingRentId { get; set; }

    public RentModel OngoingRent { get; set; }

    public ICollection<RentModel> Rents { get; set; }

    public class Configuration : IEntityTypeConfiguration<CustomerModel>
    {
        public void Configure(EntityTypeBuilder<CustomerModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.OngoingRent)
                .WithOne()
                .HasForeignKey<CustomerModel>(x => x.OngoingRentId);
        }
    }
}
