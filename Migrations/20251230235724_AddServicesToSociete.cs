using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestation.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesToSociete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Prestataires_IdPrestataire",
                table: "Prestations");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BillingType",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedPrestations",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Deliverables",
                table: "Services",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxConcurrentPrestations",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Services",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SkillsRequired",
                table: "Services",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TVA",
                table: "Services",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalPrestations",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IdPrestataire",
                table: "Prestations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateDebut",
                table: "Prestations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ClientFeedback",
                table: "Prestations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientRating",
                table: "Prestations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAssignation",
                table: "Prestations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateValidation",
                table: "Prestations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateValidationQualite",
                table: "Prestations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeEstimeeHeures",
                table: "Prestations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PrestataireNotes",
                table: "Prestations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgressUpdates",
                table: "Prestations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QualiteValidee",
                table: "Prestations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RapportFinal",
                table: "Prestations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidateurQualite",
                table: "Prestations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Prestataires_IdPrestataire",
                table: "Prestations",
                column: "IdPrestataire",
                principalTable: "Prestataires",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestations_Prestataires_IdPrestataire",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "BillingType",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CompletedPrestations",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Deliverables",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "MaxConcurrentPrestations",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "SkillsRequired",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TVA",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TotalPrestations",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ClientFeedback",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "ClientRating",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DateAssignation",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DateValidation",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DateValidationQualite",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "DureeEstimeeHeures",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "PrestataireNotes",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "ProgressUpdates",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "QualiteValidee",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "RapportFinal",
                table: "Prestations");

            migrationBuilder.DropColumn(
                name: "ValidateurQualite",
                table: "Prestations");

            migrationBuilder.AlterColumn<int>(
                name: "IdPrestataire",
                table: "Prestations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateDebut",
                table: "Prestations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestations_Prestataires_IdPrestataire",
                table: "Prestations",
                column: "IdPrestataire",
                principalTable: "Prestataires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
