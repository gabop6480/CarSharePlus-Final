using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSharePlus.Migrations
{
    /// <inheritdoc />
    public partial class CambiarPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Usuarios",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Usuarios",
                newName: "PasswordHash");
        }
    }
}
