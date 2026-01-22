using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSharePlus.Migrations
{
    /// <inheritdoc />
    public partial class PrecisionFixForTarifa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaPorHora",
                table: "Vehiculos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPago",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoPago",
                table: "Reservas");

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaPorHora",
                table: "Vehiculos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }
    }
}
