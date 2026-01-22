using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSharePlus.Migrations
{
    /// <inheritdoc />
    public partial class AddVehiculoCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutonomiaKm",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ConsumoPorKm",
                table: "Vehiculos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Energia",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Transmision",
                table: "Vehiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutonomiaKm",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "ConsumoPorKm",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Energia",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "Transmision",
                table: "Vehiculos");
        }
    }
}
