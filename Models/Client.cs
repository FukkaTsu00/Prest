
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class Client
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

        [StringLength(255)]
        public string? Adresse { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string TypeClient { get; set; } = "Particulier";

        // Navigation
        public ICollection<Contrat>? Contrats { get; set; }
    }

}