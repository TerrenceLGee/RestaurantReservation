using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RestaurantReservation.Domain.Reservations;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration;

public class ReservationTableConfiguration : IEntityTypeConfiguration<ReservationTable>
{
    public void Configure(EntityTypeBuilder<ReservationTable> builder)
    {
        builder.ToTable("reservation_tables");
        builder.HasKey(rt => new { rt.ReservationId, rt.TableId });

        builder.HasOne(rt => rt.Reservation)
            .WithMany(r => r.Tables)
            .HasForeignKey(rt => rt.ReservationId);

        builder.HasOne(rt => rt.Table)
            .WithMany(r => r.Reservations)
            .HasForeignKey(rt => rt.TableId);
    }
}