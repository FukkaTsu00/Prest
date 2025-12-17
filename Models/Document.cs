
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
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdPrestation { get; set; }

        [Required]
        public int IdTypeDocument { get; set; }

        [Required, StringLength(255)]
        public string NomDocument { get; set; } = null!;

        [ForeignKey("IdPrestation")]
        public Prestation? Prestation { get; set; }

        [ForeignKey("IdTypeDocument")]
        public TypeDocument? TypeDocument { get; set; }
    }
}