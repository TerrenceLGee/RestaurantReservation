using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration;

public class TableGroupConfiguration : IEntityTypeConfiguration<TableGroup>
{
    public void Configure(EntityTypeBuilder<TableGroup> builder)
    {
        builder.ToTable("table_groups");
        builder.HasKey(tg => tg.Id);

        builder.HasOne(tg => tg.Restaurant)
            .WithMany(r => r.TableGroups)
            .HasForeignKey(tg => tg.RestaurantId);
    }
}