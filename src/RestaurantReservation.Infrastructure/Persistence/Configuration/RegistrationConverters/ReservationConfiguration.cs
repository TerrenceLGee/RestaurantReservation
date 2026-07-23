using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;
using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

namespace RestaurantReservation.Infrastructure.Persistence.Configuration.RegistrationConverters;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .HasConversion<string>();

        builder.HasOne(rs => rs.Restaurant)
            .WithMany(r => r.Reservations)
            .HasForeignKey(rs => rs.RestaurantId);

        builder.HasOne(rs => rs.Customer)
            .WithMany(c => c.Reservations)
            .HasForeignKey(rs => rs.CustomerId);

        builder.ComplexProperty<CustomerContactInfo>(r => r.CustomerContactInfo, ci =>
        {
            ci.Property(i => i.FirstName)
                .HasMaxLength(50)
                .HasConversion(new FirstNameConverter())
                .HasColumnName("customer_first_name");

            ci.Property(i => i.LastName)
                .HasMaxLength(50)
                .HasConversion(new LastNameConverter())
                .HasColumnName("customer_last_name");

            ci.Property(i => i.EmailAddress)
                .HasMaxLength(50)
                .HasConversion(new EmailAddressConverter())
                .HasColumnName("customer_email_address");

            ci.Property(i => i.TelephoneNumber)
                .HasMaxLength(15)
                .HasConversion(new TelephoneNumberConverter())
                .HasColumnName("customer_telephone_number");
        });

        builder.ComplexProperty<ReservationInfo>(r => r.ReservationInfo, ri =>
        {
            ri.Property(i => i.Date)
                .HasConversion(new ReservationDateConverter())
                .HasColumnName("reservation_date");

            ri.Property(i => i.StartTime)
                .HasConversion(new ReservationStartConverter())
                .HasColumnName("reservation_start_time");

            ri.Property(i => i.EndTime)
                .HasConversion(new ReservationEndConverter())
                .HasColumnName("reservation_end_time");

            ri.Property(i => i.Guests)
                .HasConversion(new GuestsInPartyConverter())
                .HasColumnName("number_of_guests");
        });
    }
}