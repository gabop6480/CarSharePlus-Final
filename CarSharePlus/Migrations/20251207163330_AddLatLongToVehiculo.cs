using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSharePlus.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLongToVehiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "Vehiculos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "Vehiculos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Vehiculos");
        }
    }
}
