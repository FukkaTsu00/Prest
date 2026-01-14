using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestation.Models
{
    public class TypeDocument
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Libelle { get; set; } = null!;

        public string? Description { get; set; }

        public bool Obligatoire { get; set; } = false;

        [StringLength(100)]
        public string? Categorie { get; set; }

        // Navigation
        public ICollection<Document>? Documents { get; set; }
    }

}