using Microsoft.EntityFrameworkCore;
using GestionPrestation.Data;
using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public class PrestataireService : IPrestataireService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrestataireService> _logger;

        public PrestataireService(ApplicationDbContext context, ILogger<PrestataireService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Prestataire?> GetPrestataireByUserIdAsync(string userId)
        {
            return await _context.Prestataires
                .Include(p => p.Prestations)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public async Task<Prestataire?> GetPrestataireByIdAsync(int id)
        {
            return await _context.Prestataires
                .Include(p => p.Prestations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdatePrestataireAsync(Prestataire prestataire, string userId)
        {
            var existing = await GetPrestataireByUserIdAsync(userId);
            if (existing == null) return false;

            existing.Nom = prestataire.Nom;
            existing.Prenom = prestataire.Prenom;
            existing.Telephone = prestataire.Telephone;
            existing.Specialite = prestataire.Specialite;
            existing.TarifHoraire = prestataire.TarifHoraire;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(int prestataireId, bool disponible)
        {
            var prestataire = await _context.Prestataires.FindAsync(prestataireId);
            if (prestataire == null) return false;

            prestataire.Disponible = disponible;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalEarningsAsync(int prestataireId)
        {
            return await _context.Prestations
                .Where(p => p.IdPrestataire == prestataireId && 
                           (p.Statut == PrestationStatus.Validee || p.Statut == PrestationStatus.Terminee))
                .SumAsync(p => p.PrixFinal);
        }

        public async Task<int> GetCompletedPrestationsCountAsync(int prestataireId)
        {
            return await _context.Prestations
                .CountAsync(p => p.IdPrestataire == prestataireId && 
                               (p.Statut == PrestationStatus.Validee || p.Statut == PrestationStatus.Terminee));
        }

        public async Task<decimal> GetAverageRatingAsync(int prestataireId)
        {
            var ratings = await _context.Prestations
                .Where(p => p.IdPrestataire == prestataireId && p.ClientRating.HasValue)
                .Select(p => p.ClientRating!.Value)
                .ToListAsync();

            return ratings.Any() ? (decimal)ratings.Average() : 0;
        }

        public async Task<List<Prestation>> GetPerformanceMetricsAsync(int prestataireId)
        {
            return await _context.Prestations
                .Include(p => p.Client)
                .Include(p => p.Service)
                .Where(p => p.IdPrestataire == prestataireId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }

        public async Task<bool> RespondToReviewAsync(int prestationId, int prestataireId, string response)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdPrestataire != prestataireId) return false;

            prestation.PrestataireNotes = (prestation.PrestataireNotes ?? "") + $"\nResponse to review: {response}";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}









