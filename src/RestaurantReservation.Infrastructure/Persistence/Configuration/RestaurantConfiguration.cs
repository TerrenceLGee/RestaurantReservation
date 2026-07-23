using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurant");
        builder.HasKey(r => r.Id);

        builder.OwnsMany(r => r.Schedule, s =>
        {
            s.ToJson();
        });

        builder.HasIndex(r => r.Name);
    }
}