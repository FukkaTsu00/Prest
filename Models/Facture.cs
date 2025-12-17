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

    public class Facture
    {
        public int Id { get; set; }

        [Required]
        public int IdPrestation { get; set; }

        [Required, StringLength(100)]
        public string NumeroFacture { get; set; } = null!;

        public DateTime DateEmission { get; set; } = DateTime.Now;

        public DateTime? DateEcheance { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontantHT { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TauxTVA { get; set; } = 20.00M;

        [NotMapped]
        public decimal MontantTVA => MontantHT * (TauxTVA / 100);

        [NotMapped]
        public decimal MontantTTC => MontantHT + MontantTVA;

        [StringLength(50)]
        public string Statut { get; set; } = "En attente";

        [ForeignKey("IdPrestation")]
        public Prestation? Prestation { get; set; }
    }
}