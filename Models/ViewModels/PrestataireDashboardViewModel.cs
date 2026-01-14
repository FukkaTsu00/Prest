using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionPrestation.Models;

namespace GestionPrestation.Models.ViewModels
{
    public class PrestataireDashboardViewModel
    {
        public Prestataire Prestataire { get; set; } = null!;
        public List<Prestation> ActivePrestations { get; set; } = new();
        public List<Facture> RecentFactures { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int PendingServices { get; set; }
        public int CompletedServices { get; set; }
    }
}
