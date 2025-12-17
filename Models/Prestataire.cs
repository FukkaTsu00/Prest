
using GestionPrestation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class Prestataire
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Identity relation
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required, StringLength(100)]
        public string Nom { get; set; } = null!;

        [Required, StringLength(100)]
        public string Prenom { get; set; } = null!;

        [Required, StringLength(20)]
        public string Telephone { get; set; } = null!;

        [StringLength(100)]
        public string? Specialite { get; set; }

        public decimal TarifHoraire { get; set; }

        public bool Disponible { get; set; } = true;

        public DateTime DateInscription { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Prestation>? Prestations { get; set; }
    }
}