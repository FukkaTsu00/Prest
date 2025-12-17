using GestionPrestation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class Societe
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nom { get; set; } = null!;

        [StringLength(255)]
        public string Adresse { get; set; } = null!;

        [Required, StringLength(100)]
        public string Email { get; set; } = null!;

        [Required, StringLength(50)]
        public string NumeroStringCommerce { get; set; } = null!;

        // Navigation
        public ICollection<Prestation>? Prestations { get; set; }
    }
}