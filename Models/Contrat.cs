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
    public class Contrat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdClient { get; set; }

        [Required]
        public int IdPrestation { get; set; }

        [Required, StringLength(100)]
        public string NumeroContrat { get; set; } = null!;

        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontantTotal { get; set; }

        [StringLength(50)]
        public string Statut { get; set; } = "Actif";

        [ForeignKey("IdClient")]
        public Client? Client { get; set; }

        [ForeignKey("IdPrestation")]
        public Prestation? Prestation { get; set; }
    }
}