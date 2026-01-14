using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestation.Migrations
{
    /// <inheritdoc />
    public partial class updatesociete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Societes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Societes_ApplicationUserId",
                table: "Societes",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Societes_AspNetUsers_ApplicationUserId",
                table: "Societes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Societes_AspNetUsers_ApplicationUserId",
                table: "Societes");

            migrationBuilder.DropIndex(
                name: "IX_Societes_ApplicationUserId",
                table: "Societes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Societes");
        }
    }
}
