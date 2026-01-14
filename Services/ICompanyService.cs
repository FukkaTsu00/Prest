using GestionPrestation.Models;
using GestionPrestation.Models.ViewModels;

namespace GestionPrestation.Services
{
    public interface ICompanyService
    {
        Task<bool> RegisterCompanyAsync(Societe societe, string userId);
        Task<Societe?> GetCompanyByUserIdAsync(string userId);
        Task<Societe?> GetCompanyByIdAsync(int id);
        Task<bool> UpdateCompanyAsync(Societe societe, string userId);
        Task<bool> RegisterPrestataireAsync(Prestataire prestataire, int societeId, string userId);
        Task<List<Prestataire>> GetCompanyPrestatairesAsync(int societeId);
        Task<List<Service>> GetCompanyServicesAsync(int societeId);
        Task<Service?> CreateServiceAsync(Service service, int societeId);
        Task<bool> UpdateServiceAsync(Service service, int societeId);
        Task<bool> DeleteServiceAsync(int serviceId, int societeId);
        Task<CompanyDashboardViewModel> GetCompanyDashboardAsync(int societeId);
        Task<List<Prestation>> GetCompanyPerformanceAsync(int societeId);
    }
}
