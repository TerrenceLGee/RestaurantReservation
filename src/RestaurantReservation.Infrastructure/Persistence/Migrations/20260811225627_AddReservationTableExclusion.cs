using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationTableExclusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist");

            migrationBuilder.Sql(@"
                    ALTER TABLE reservation_tables 
                    ADD CONSTRAINT no_overlapping_table_reservations 
                    EXCLUDE USING gist (
                        table_id WITH =,
                        tsrange(
                        scheduled_reservation_reservation_day + scheduled_reservation_reservation_start,
                        scheduled_reservation_reservation_day + scheduled_reservation_reservation_end
                        ) WITH && 
                    );
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE reservation_tables 
                DROP CONSTRAINT no_overlapping_table_reservations;
                ");
        }
    }
}
