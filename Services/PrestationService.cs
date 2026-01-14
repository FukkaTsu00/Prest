using Microsoft.EntityFrameworkCore;
using GestionPrestation.Data;
using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public class PrestationService : IPrestationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrestationService> _logger;

        public PrestationService(ApplicationDbContext context, ILogger<PrestationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Client operations
        public async Task<Prestation?> CreatePrestationRequestAsync(int serviceId, int clientId, string description)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null) return null;

                var prestation = new Prestation
                {
                    IdService = serviceId,
                    IdClient = clientId,
                    Description = description,
                    Statut = PrestationStatus.Planifiee,
                    PrixFinal = service.PrixBase,
                    DureeEstimeeHeures = service.DureeEstimeeHeures,
                    DateCreation = DateTime.Now
                };

                _context.Prestations.Add(prestation);
                await _context.SaveChangesAsync();
                return prestation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prestation request");
                return null;
            }
        }

        public async Task<List<Prestation>> GetClientPrestationsAsync(int clientId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Prestataire)
                .Where(p => p.IdClient == clientId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }

        public async Task<Prestation?> GetPrestationByIdAsync(int id)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .ThenInclude(s => s.Societe)
                .Include(p => p.Client)
                .Include(p => p.Prestataire)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> RatePrestationAsync(int prestationId, int rating, string? feedback, int clientId)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdClient != clientId) return false;

            prestation.ClientRating = rating;
            prestation.ClientFeedback = feedback;
            if (prestation.Statut == PrestationStatus.Terminee)
            {
                prestation.Statut = PrestationStatus.Validee;
                prestation.DateValidation = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPrestationAsync(int prestationId, int clientId)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdClient != clientId) return false;
            if (prestation.CanTransitionTo(PrestationStatus.Annulee))
            {
                prestation.Statut = PrestationStatus.Annulee;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Prestataire operations
        public async Task<List<Prestation>> GetAssignedPrestationsAsync(int prestataireId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Client)
                .Where(p => p.IdPrestataire == prestataireId)
                .OrderByDescending(p => p.DateAssignation)
                .ToListAsync();
        }

        public async Task<List<Prestation>> GetAvailablePrestationsAsync(int prestataireId)
        {
            var prestataire = await _context.Prestataires
                .Include(p => p.Prestations)
                .FirstOrDefaultAsync(p => p.Id == prestataireId);
            
            if (prestataire == null) return new List<Prestation>();

            return await _context.Prestations
                .Include(p => p.Service)
                .Where(p => p.Statut == PrestationStatus.Planifiee && 
                           p.Service != null &&
                           (string.IsNullOrEmpty(prestataire.Specialite) || 
                            p.Service.SkillsRequired != null && 
                            p.Service.SkillsRequired.Contains(prestataire.Specialite)))
                .ToListAsync();
        }

        public async Task<bool> AcceptPrestationAsync(int prestationId, int prestataireId)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdPrestataire != prestataireId) return false;
            if (prestation.CanTransitionTo(PrestationStatus.Assignee))
            {
                prestation.Statut = PrestationStatus.Assignee;
                prestation.DateAssignation = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RefusePrestationAsync(int prestationId, int prestataireId)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdPrestataire != prestataireId) return false;
            prestation.IdPrestataire = null;
            prestation.Statut = PrestationStatus.Planifiee;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePrestationStatusAsync(int prestationId, int prestataireId, string status, string? notes)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdPrestataire != prestataireId) return false;
            if (!prestation.CanTransitionTo(status)) return false;

            prestation.Statut = status;
            prestation.PrestataireNotes = notes;
            
            if (status == PrestationStatus.EnCours && !prestation.DateDebut.HasValue)
                prestation.DateDebut = DateTime.Now;
            if (status == PrestationStatus.Terminee && !prestation.DateFin.HasValue)
                prestation.DateFin = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProgressAsync(int prestationId, int prestataireId, string progressUpdate)
        {
            var prestation = await _context.Prestations.FindAsync(prestationId);
            if (prestation == null || prestation.IdPrestataire != prestataireId) return false;
            
            prestation.ProgressUpdates = (prestation.ProgressUpdates ?? "") + $"\n{DateTime.Now}: {progressUpdate}";
            await _context.SaveChangesAsync();
            return true;
        }

        // Company operations
        public async Task<bool> AssignPrestationToPrestataireAsync(int prestationId, int prestataireId, int companyId)
        {
            var prestation = await GetPrestationByIdAsync(prestationId);
            if (prestation?.Service?.IdSociete != companyId) return false;

            prestation.IdPrestataire = prestataireId;
            prestation.Statut = PrestationStatus.Assignee;
            prestation.DateAssignation = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Prestation>> GetCompanyPrestationsAsync(int societeId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Client)
                .Include(p => p.Prestataire)
                .Where(p => p.Service != null && p.Service.IdSociete == societeId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }

        public async Task<bool> UpdatePrestationPricingAsync(int prestationId, decimal prixFinal, int companyId)
        {
            var prestation = await GetPrestationByIdAsync(prestationId);
            if (prestation?.Service?.IdSociete != companyId) return false;

            prestation.PrixFinal = prixFinal;
            await _context.SaveChangesAsync();
            return true;
        }

        // Admin operations
        public async Task<List<Prestation>> GetAllPrestationsAsync()
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .ThenInclude(s => s.Societe)
                .Include(p => p.Client)
                .Include(p => p.Prestataire)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }
    }
}









