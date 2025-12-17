
using GestionPrestation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class Prestation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdSociete { get; set; }

        [Required]
        public int IdPrestataire { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; }

        public string? Description { get; set; }

        [StringLength(50)]
        public string Statut { get; set; } = "Planifiée";

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontantTotal { get; set; }

        public int DureeHeures { get; set; }

        // Navigation
        [ForeignKey("IdSociete")]
        public Societe? Societe { get; set; }

        [ForeignKey("IdPrestataire")]
        public Prestataire? Prestataire { get; set; }

        public ICollection<Paiement>? Paiements { get; set; }
        public ICollection<Facture>? Factures { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ICollection<Contrat>? Contrats { get; set; }
    }
}