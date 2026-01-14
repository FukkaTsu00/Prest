using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPrestation.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Categorie { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrixBase { get; set; }

        [Required]
        public int DureeEstimeeHeures { get; set; }

        // Service management
        public bool IsActive { get; set; } = true;
        public bool RequiresApproval { get; set; } = false;
        public int? MaxConcurrentPrestations { get; set; }

        // Quality and requirements
        [StringLength(1000)]
        public string? Requirements { get; set; }

        [StringLength(1000)]
        public string? Deliverables { get; set; }

        [StringLength(500)]
        public string? SkillsRequired { get; set; }

        // Pricing and billing
        [Column(TypeName = "decimal(5,2)")]
        public decimal TVA { get; set; } = 20.00M;

        public string? BillingType { get; set; } = "Fixed"; // Fixed, Hourly, Milestone

        // Performance tracking
        public int TotalPrestations { get; set; } = 0;
        public int CompletedPrestations { get; set; } = 0;
        public decimal AverageRating { get; set; } = 0;

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? LastModified { get; set; }
        public string? ModifiedBy { get; set; }

        // Foreign Keys
        [Required]
        public int IdSociete { get; set; }

        // Navigation Properties
        [ForeignKey("IdSociete")]
        public Societe? Societe { get; set; }

        public virtual ICollection<Prestation> Prestations { get; set; } = new List<Prestation>();

        // Computed properties
        [NotMapped]
        public decimal CompletionRate => TotalPrestations > 0 ? (decimal)CompletedPrestations / TotalPrestations * 100 : 0;

        [NotMapped]
        public bool IsAvailable => IsActive && (MaxConcurrentPrestations == null || 
            Prestations.Count(p => p.Statut == "En cours") < MaxConcurrentPrestations.Value);

        [NotMapped]
        public decimal TTCPrice => PrixBase * (1 + TVA / 100);

        // Service categories
        public static class ServiceCategories
        {
            public const string Consulting = "Consulting";
            public const string Development = "Development";
            public const string Design = "Design";
            public const string Marketing = "Marketing";
            public const string Support = "Support";
            public const string Training = "Training";
            public const string Maintenance = "Maintenance";
            public const string Other = "Other";
        }

        // Billing types
        public static class BillingTypes
        {
            public const string Fixed = "Fixed";
            public const string Hourly = "Hourly";
            public const string Milestone = "Milestone";
            public const string Subscription = "Subscription";
        }
    }
}
