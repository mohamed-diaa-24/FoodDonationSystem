using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDonationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeAnyClacluatedColumninDomins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDeliveries",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "TotalDonations",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "TotalReceived",
                table: "Charities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedDeliveries",
                table: "Volunteers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalDonations",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalReceived",
                table: "Charities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
