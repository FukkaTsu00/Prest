using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public interface IClientService
    {
        Task<Client?> GetClientByUserIdAsync(string userId);
        Task<Client?> GetClientByIdAsync(int id);
        Task<bool> UpdateClientAsync(Client client, string userId);
        Task<List<Prestation>> GetClientPrestationHistoryAsync(int clientId);
        Task<List<Prestation>> GetClientActivePrestationsAsync(int clientId);
    }
}
