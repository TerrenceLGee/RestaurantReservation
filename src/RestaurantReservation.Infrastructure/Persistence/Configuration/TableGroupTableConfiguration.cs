using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration;

public class TableGroupTableConfiguration : IEntityTypeConfiguration<TableGroupTable>
{
    public void Configure(EntityTypeBuilder<TableGroupTable> builder)
    {
        builder.ToTable("table_group_tables");
        builder.HasKey(tgt => new { tgt.TableId, tgt.TableGroupId });

        builder.HasOne(tgt => tgt.Table)
            .WithMany(t => t.TableGroups)
            .HasForeignKey(tgt => tgt.TableId);

        builder.HasOne(tgt => tgt.TableGroup)
            .WithMany(tg => tg.Tables)
            .HasForeignKey(tgt => tgt.TableGroupId);
    }
}