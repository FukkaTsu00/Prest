using GestionPrestation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPrestation.Models
{
    public class Prestation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdService { get; set; }

        [Required]
        public int IdClient { get; set; }

        // Prestataire assignment (nullable until assigned)
        public int? IdPrestataire { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Timeline tracking
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateAssignation { get; set; }
        public DateTime? DateValidation { get; set; }

        // Status tracking with proper workflow
        [StringLength(50)]
        public string Statut { get; set; } = "Planifiée";

        // Quality and performance metrics
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrixFinal { get; set; }

        public int DureeEstimeeHeures { get; set; }
        public int DureeReelleHeures { get; set; }

        // Client feedback
        public int? ClientRating { get; set; } // 1-5 stars
        public string? ClientFeedback { get; set; }

        // Prestataire reports
        public string? PrestataireNotes { get; set; }
        public string? RapportFinal { get; set; }

        // Communication and progress tracking
        public string? Notes { get; set; }
        public string? ProgressUpdates { get; set; }

        // Description for the service
        [StringLength(500)]
        public string? Description { get; set; }

        // Quality control
        public bool QualiteValidee { get; set; } = false;
        public string? ValidateurQualite { get; set; }
        public DateTime? DateValidationQualite { get; set; }

        // Navigation Properties
        [ForeignKey("IdService")]
        public Service? Service { get; set; }

        [ForeignKey("IdClient")]
        public Client? Client { get; set; }

        [ForeignKey("IdPrestataire")]
        public Prestataire? Prestataire { get; set; }

        // The Societe comes from the Service relationship
        public Societe? Societe => Service?.Societe;

        public ICollection<Paiement>? Paiements { get; set; }
        public ICollection<Facture>? Factures { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ICollection<Contrat>? Contrats { get; set; }

        // Computed properties
        [NotMapped]
        public bool IsAssigned => IdPrestataire.HasValue;

        [NotMapped]
        public bool IsCompleted => Statut == "Terminée" || Statut == "Validée";

        [NotMapped]
        public bool IsInProgress => Statut == "En cours";

        [NotMapped]
        public bool IsDelayed => DateFin.HasValue && DateFin.Value < DateTime.Now && !IsCompleted;

        [NotMapped]
        public decimal PerformanceRate => DureeReelleHeures > 0 ? (decimal)DureeEstimeeHeures / DureeReelleHeures * 100 : 0;

        // Status workflow validation
        public bool CanTransitionTo(string newStatus)
        {
            return newStatus switch
            {
                "Assignée" => Statut == "Planifiée" && IdPrestataire.HasValue,
                "En cours" => Statut == "Assignée",
                "Terminée" => Statut == "En cours",
                "Validée" => Statut == "Terminée" && ClientRating.HasValue,
                "Annulée" => Statut != "Terminée" && Statut != "Validée",
                _ => false
            };
        }
    }

    // Prestation status constants
    public static class PrestationStatus
    {
        public const string Planifiee = "Planifiée";
        public const string Assignee = "Assignée";
        public const string EnCours = "En cours";
        public const string Terminee = "Terminée";
        public const string Validee = "Validée";
        public const string Annulee = "Annulée";
        public const string Retard = "En retard";
        public const string EnPause = "En pause";
    }
}