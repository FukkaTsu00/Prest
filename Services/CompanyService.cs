using Microsoft.EntityFrameworkCore;
using GestionPrestation.Data;
using GestionPrestation.Models;
using GestionPrestation.Models.ViewModels;

namespace GestionPrestation.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ApplicationDbContext context, ILogger<CompanyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> RegisterCompanyAsync(Societe societe, string userId)
        {
            try
            {
                societe.ApplicationUserId = userId;
                _context.Societes.Add(societe);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering company");
                return false;
            }
        }

        public async Task<Societe?> GetCompanyByUserIdAsync(string userId)
        {
            return await _context.Societes
                .Include(s => s.Services)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
        }

        public async Task<Societe?> GetCompanyByIdAsync(int id)
        {
            return await _context.Societes
                .Include(s => s.Services)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateCompanyAsync(Societe societe, string userId)
        {
            var existing = await GetCompanyByUserIdAsync(userId);
            if (existing == null) return false;

            existing.Nom = societe.Nom;
            existing.Adresse = societe.Adresse;
            existing.Email = societe.Email;
            existing.NumeroStringCommerce = societe.NumeroStringCommerce;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterPrestataireAsync(Prestataire prestataire, int societeId, string userId)
        {
            try
            {
                var societe = await _context.Societes.FindAsync(societeId);
                if (societe == null || societe.ApplicationUserId != userId) return false;

                prestataire.ApplicationUserId = userId;
                _context.Prestataires.Add(prestataire);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering prestataire");
                return false;
            }
        }

        public async Task<List<Prestataire>> GetCompanyPrestatairesAsync(int societeId)
        {
            // Return all approved prestataires that companies can assign to
            // In a more advanced system, you might want to link prestataires to specific companies
            return await _context.Prestataires
                .Where(p => p.IsApproved)
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prenom)
                .ToListAsync();
        }

        public async Task<List<Service>> GetCompanyServicesAsync(int societeId)
        {
            return await _context.Services
                .Where(s => s.IdSociete == societeId)
                .Include(s => s.Prestations)
                .ToListAsync();
        }

        public async Task<Service?> CreateServiceAsync(Service service, int societeId)
        {
            try
            {
                service.IdSociete = societeId;
                service.DateCreation = DateTime.Now;
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return service;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                return null;
            }
        }

        public async Task<bool> UpdateServiceAsync(Service service, int societeId)
        {
            var existing = await _context.Services.FindAsync(service.Id);
            if (existing == null || existing.IdSociete != societeId) return false;

            existing.Nom = service.Nom;
            existing.Description = service.Description;
            existing.Categorie = service.Categorie;
            existing.PrixBase = service.PrixBase;
            existing.DureeEstimeeHeures = service.DureeEstimeeHeures;
            existing.IsActive = service.IsActive;
            existing.LastModified = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int serviceId, int societeId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null || service.IdSociete != societeId) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CompanyDashboardViewModel> GetCompanyDashboardAsync(int societeId)
        {
            var services = await GetCompanyServicesAsync(societeId);
            var prestations = await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Client)
                .Include(p => p.Prestataire)
                .Where(p => p.Service != null && p.Service.IdSociete == societeId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();

            return new CompanyDashboardViewModel
            {
                Societe = await GetCompanyByIdAsync(societeId),
                TotalServices = services.Count,
                ActivePrestations = prestations.Count(p => p.Statut == PrestationStatus.EnCours || p.Statut == PrestationStatus.Assignee),
                TotalRevenue = prestations
                    .Where(p => p.Statut == PrestationStatus.Validee || p.Statut == PrestationStatus.Terminee)
                    .Sum(p => p.PrixFinal),
                RecentPrestations = prestations.Take(10).ToList()
            };
        }

        public async Task<List<Prestation>> GetCompanyPerformanceAsync(int societeId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Client)
                .Include(p => p.Prestataire)
                .Where(p => p.Service != null && p.Service.IdSociete == societeId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }
    }

    public class CompanyDashboardViewModel
    {
        public Societe? Societe { get; set; }
        public int TotalServices { get; set; }
        public int ActivePrestations { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Prestation> RecentPrestations { get; set; } = new();
    }
}




