using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionPrestation.Models;

namespace GestionPrestation.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalClients { get; set; }
        public int TotalPrestataires { get; set; }
        public int TotalSocietes { get; set; }
        public int TotalPrestations { get; set; }
        public int ActivePrestations { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingInvoices { get; set; }
        public List<Prestation> RecentPrestations { get; set; } = new();
        public List<Facture> RecentInvoices { get; set; } = new();
    }

    public class AnalyticsViewModel
    {
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new();
        public Dictionary<string, int> PrestationsByStatus { get; set; } = new();
        public List<TopPrestataire> TopPrestataires { get; set; } = new();
        public List<RecentActivity> RecentActivity { get; set; } = new();
    }

    public class TopPrestataire
    {
        public Prestataire Prestataire { get; set; } = null!;
        public decimal Revenue { get; set; }
        public int ServicesCount { get; set; }
    }

    public class RecentActivity
    {
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
    }
}
