using GestionPrestation.Models;
using GestionPrestation.Models.ViewModels;

namespace GestionPrestation.Services
{
    public interface IUserManagementService
    {
        Task<List<UserWithRoles>> GetAllUsersAsync();
        Task<UserWithRoles?> GetUserByIdAsync(string userId);
        Task<bool> CreateUserAsync(ApplicationUser user, string password, string role);
        Task<bool> UpdateUserAsync(string id, ApplicationUser updatedUser);
        Task<bool> DeactivateUserAsync(string id);
        Task<bool> ActivateUserAsync(string id);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<List<Societe>> GetPendingCompaniesAsync();
        Task<List<Prestataire>> GetPendingPrestatairesAsync();
        Task<bool> ApproveCompanyAsync(int societeId);
        Task<bool> RejectCompanyAsync(int societeId);
        Task<bool> ApprovePrestataireAsync(int prestataireId);
        Task<bool> RejectPrestataireAsync(int prestataireId);
    }
}
