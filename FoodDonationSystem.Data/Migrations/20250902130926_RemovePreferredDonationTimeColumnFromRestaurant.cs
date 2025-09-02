using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDonationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePreferredDonationTimeColumnFromRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredDonationTime",
                table: "Restaurants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "PreferredDonationTime",
                table: "Restaurants",
                type: "time",
                nullable: true);
        }
    }
}
