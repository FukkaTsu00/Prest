using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public interface IPrestationService
    {
        Task<Prestation?> CreatePrestationRequestAsync(int serviceId, int clientId, string description);
        Task<List<Prestation>> GetClientPrestationsAsync(int clientId);
        Task<Prestation?> GetPrestationByIdAsync(int prestationId);
        Task<bool> RatePrestationAsync(int prestationId, int rating, string? feedback, int clientId);
        Task<bool> CancelPrestationAsync(int prestationId, int clientId);
        Task<List<Prestation>> GetAssignedPrestationsAsync(int prestataireId);
        Task<List<Prestation>> GetAvailablePrestationsAsync(int prestataireId);
        Task<bool> AcceptPrestationAsync(int prestationId, int prestataireId);
        Task<bool> RefusePrestationAsync(int prestationId, int prestataireId);
        Task<bool> UpdatePrestationStatusAsync(int prestationId, int prestataireId, string status, string? notes);
        Task<bool> UpdateProgressAsync(int prestationId, int prestataireId, string progressUpdate);
        Task<bool> AssignPrestationToPrestataireAsync(int prestationId, int prestataireId, int companyId);
        Task<List<Prestation>> GetCompanyPrestationsAsync(int societeId);
        Task<bool> UpdatePrestationPricingAsync(int prestationId, decimal prixFinal, int companyId);
        Task<List<Prestation>> GetAllPrestationsAsync();
    }
}
