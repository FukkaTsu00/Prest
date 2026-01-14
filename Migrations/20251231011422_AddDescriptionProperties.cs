using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestation.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prestations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Contrats",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Contrats");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");
        }
    }
}
