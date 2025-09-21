using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDonationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeliveryVolunteerFromReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deliveries_ReservationId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "VolunteerId",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ReservationId",
                table: "Deliveries",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deliveries_ReservationId",
                table: "Deliveries");

            migrationBuilder.AddColumn<int>(
                name: "VolunteerId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ReservationId",
                table: "Deliveries",
                column: "ReservationId",
                unique: true);
        }
    }
}
