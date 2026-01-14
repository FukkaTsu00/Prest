using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestation.Migrations
{
    /// <inheritdoc />
    public partial class work : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Societes_IdSociete",
                table: "Prestations");

            migrationBuilder.RenameColumn(
                name: "MontantTotal",
                table: "Prestations",
                newName: "PrixFinal");

            migrationBuilder.RenameColumn(
                name: "IdSociete",
                table: "Prestations",
                newName: "IdService");

            migrationBuilder.RenameColumn(
                name: "DureeHeures",
                table: "Prestations",
                newName: "IdClient");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Prestations",
                newName: "Notes");

            migrationBuilder.RenameIndex(
                name: "IX_Prestations_IdSociete",
                table: "Prestations",
                newName: "IX_Prestations_IdService");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "Prestations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DureeReelleHeures",
                table: "Prestations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SocieteId",
                table: "Prestations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrixBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DureeEstimeeHeures = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdSociete = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Societes_IdSociete",
                        column: x => x.IdSociete,
                        principalTable: "Societes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prestations_IdClient",
                table: "Prestations",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Prestations_SocieteId",
                table: "Prestations",
                column: "SocieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_IdSociete",
                table: "Services",
                column: "IdSociete");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Clients_IdClient",
                table: "Prestations",
                column: "IdClient",
                principalTable: "Clients",
                principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Services_IdService",
                table: "Prestations",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Societes_SocieteId",
                table: "Prestations",
                column: "SocieteId",
                principalTable: "Societes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Clients_IdClient",
                table: "Prestations");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Services_IdService",
                table: "Prestations");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Societes_SocieteId",
                table: "Prestations");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Prestations_IdClient",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "IX_Prestations_SocieteId",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DureeReelleHeures",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "SocieteId",
                table: "Prestations");

            migrationBuilder.RenameColumn(
                name: "PrixFinal",
                table: "Prestations",
                newName: "MontantTotal");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Prestations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "IdService",
                table: "Prestations",
                newName: "IdSociete");

            migrationBuilder.RenameColumn(
                name: "IdClient",
                table: "Prestations",
                newName: "DureeHeures");

            migrationBuilder.RenameIndex(
                name: "IX_Prestations_IdService",
                table: "Prestations",
                newName: "IX_Prestations_IdSociete");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Societes_IdSociete",
                table: "Prestations",
                column: "IdSociete",
                principalTable: "Societes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
