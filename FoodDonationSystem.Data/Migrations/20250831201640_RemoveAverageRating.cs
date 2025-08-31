using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDonationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAverageRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Charities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Volunteers",
                type: "float(3)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Restaurants",
                type: "float(3)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Charities",
                type: "float(3)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
