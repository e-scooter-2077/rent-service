using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EScooter.RentService.Infrastructure.DataAccess.Models
{
    public class ScooterModel
    {
        public Guid Id { get; set; }

        public Guid? RentingCustomerId { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsOutOfService { get; set; }

        public bool IsInStandby { get; set; }

        public CustomerModel RentingCustomer { get; set; }

        public class Configuration : IEntityTypeConfiguration<ScooterModel>
        {
            public void Configure(EntityTypeBuilder<ScooterModel> builder)
            {
                builder.HasKey(x => x.Id);
            }
        }
    }
}
