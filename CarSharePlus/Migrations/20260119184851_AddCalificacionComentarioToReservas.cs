using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSharePlus.Migrations
{
    /// <inheritdoc />
    public partial class AddCalificacionComentarioToReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Calificacion",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "Reservas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calificacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "Reservas");
        }
    }
}
