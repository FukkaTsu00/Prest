using Microsoft.EntityFrameworkCore;
using GestionPrestation.Data;
using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientService> _logger;

        public ClientService(ApplicationDbContext context, ILogger<ClientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Client?> GetClientByUserIdAsync(string userId)
        {
            return await _context.Clients
                .Include(c => c.Contrats)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Contrats)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateClientAsync(Client client, string userId)
        {
            var existing = await GetClientByUserIdAsync(userId);
            if (existing == null) return false;

            existing.Nom = client.Nom;
            existing.Prenom = client.Prenom;
            existing.Telephone = client.Telephone;
            existing.Email = client.Email;
            existing.Adresse = client.Adresse;
            existing.TypeClient = client.TypeClient;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Prestation>> GetClientPrestationHistoryAsync(int clientId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Prestataire)
                .Where(p => p.IdClient == clientId)
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }

        public async Task<List<Prestation>> GetClientActivePrestationsAsync(int clientId)
        {
            return await _context.Prestations
                .Include(p => p.Service)
                .Include(p => p.Prestataire)
                .Where(p => p.IdClient == clientId && 
                           (p.Statut == PrestationStatus.EnCours || 
                            p.Statut == PrestationStatus.Assignee ||
                            p.Statut == PrestationStatus.Planifiee))
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();
        }
    }
}









