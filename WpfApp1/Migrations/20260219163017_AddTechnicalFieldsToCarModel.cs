using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalFieldsToCarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrakeForceDifference",
                table: "CarModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurbMass",
                table: "CarModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxMass",
                table: "CarModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingBrake",
                table: "CarModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReserveBrake",
                table: "CarModels",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrakeForceDifference",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "CurbMass",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "MaxMass",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "ParkingBrake",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "ReserveBrake",
                table: "CarModels");
        }
    }
}
