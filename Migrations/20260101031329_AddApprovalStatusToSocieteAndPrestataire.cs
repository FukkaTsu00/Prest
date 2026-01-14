using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestation.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatusToSocieteAndPrestataire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Societes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Societes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Societes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Prestataires",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Prestataires",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Prestataires",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Societes");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Societes");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Societes");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Prestataires");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Prestataires");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Prestataires");
        }
    }
}
