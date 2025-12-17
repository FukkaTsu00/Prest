using GestionPrestation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdPrestation { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Montant { get; set; }

        public DateTime DatePaiement { get; set; } = DateTime.Now;

        [Required, StringLength(50)]
        public string MethodePaiement { get; set; } = null!;

        [StringLength(50)]
        public string Statut { get; set; } = "En attente";

        [StringLength(100)]
        public string? Reference { get; set; }

        [ForeignKey("IdPrestation")]
        public Prestation? Prestation { get; set; }
    }
}