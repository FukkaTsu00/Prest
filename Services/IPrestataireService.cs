using GestionPrestation.Models;

namespace GestionPrestation.Services
{
    public interface IPrestataireService
    {
        Task<Prestataire?> GetPrestataireByUserIdAsync(string userId);
        Task<Prestataire?> GetPrestataireByIdAsync(int id);
        Task<bool> UpdatePrestataireAsync(Prestataire prestataire, string userId);
        Task<bool> UpdateAvailabilityAsync(int prestataireId, bool disponible);
        Task<decimal> GetTotalEarningsAsync(int prestataireId);
        Task<int> GetCompletedPrestationsCountAsync(int prestataireId);
        Task<decimal> GetAverageRatingAsync(int prestataireId);
        Task<List<Prestation>> GetPerformanceMetricsAsync(int prestataireId);
        Task<bool> RespondToReviewAsync(int prestationId, int prestataireId, string response);
    }
}
