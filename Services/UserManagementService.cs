using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using GestionPrestation.Data;
using GestionPrestation.Models;
using GestionPrestation.Models.ViewModels;

namespace GestionPrestation.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<UserManagementService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserWithRoles>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userWithRoles = new UserWithRoles
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    User = user,
                    Roles = roles.ToList(),
                    RoleName = roles.FirstOrDefault() ?? "",
                    IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow
                };

                if (roles.Contains("Client"))
                    userWithRoles.Client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
                if (roles.Contains("Prestataire"))
                    userWithRoles.Prestataire = await _context.Prestataires.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
                if (roles.Contains("Societe"))
                    userWithRoles.Societe = await _context.Societes.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

                result.Add(userWithRoles);
            }
            return result;
        }

        public async Task<UserWithRoles?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            
            var roles = await _userManager.GetRolesAsync(user);
            var userWithRoles = new UserWithRoles
            {
                Id = user.Id,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                User = user,
                Roles = roles.ToList(),
                RoleName = roles.FirstOrDefault() ?? "",
                IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow
            };

            if (roles.Contains("Client"))
                userWithRoles.Client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (roles.Contains("Prestataire"))
                userWithRoles.Prestataire = await _context.Prestataires.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
            if (roles.Contains("Societe"))
                userWithRoles.Societe = await _context.Societes.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            return userWithRoles;
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password, string role)
        {
            // Ensure UserName is set (Identity requirement)
            if (string.IsNullOrEmpty(user.UserName))
            {
                user.UserName = user.Email;
            }
            
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                
                // Create associated entity based on role
                if (role == "Client")
                {
                    var client = new Client
                    {
                        ApplicationUserId = user.Id,
                        Nom = user.LastName,
                        Prenom = user.FirstName,
                        Email = user.Email ?? "",
                        Telephone = user.PhoneNumber ?? "",
                        Adresse = string.Empty,
                        TypeClient = "Particulier"
                    };
                    _context.Clients.Add(client);
                }
                else if (role == "Prestataire")
                {
                    var prestataire = new Prestataire
                    {
                        ApplicationUserId = user.Id,
                        Nom = user.LastName,
                        Prenom = user.FirstName,
                        Telephone = user.PhoneNumber ?? "",
                        Specialite = "General",
                        TarifHoraire = 0,
                        Disponible = true,
                        IsApproved = false
                    };
                    _context.Prestataires.Add(prestataire);
                }
                else if (role == "Societe")
                {
                    var societe = new Societe
                    {
                        ApplicationUserId = user.Id,
                        Nom = user.LastName,
                        Email = user.Email ?? "",
                        Adresse = string.Empty,
                        NumeroStringCommerce = "PENDING",
                        IsApproved = false
                    };
                    _context.Societes.Add(societe);
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUserAsync(string userId, ApplicationUser updatedUser)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            return true;
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _userManager.SetLockoutEndDateAsync(user, null);
            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            try
            {
                // Get user roles to determine what entities to clean up
                var userRoles = await _userManager.GetRolesAsync(user);
                
                // Delete associated entities based on role
                if (userRoles.Contains("Client"))
                {
                    var client = await _context.Clients
                        .Include(c => c.Contrats)
                        .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
                    if (client != null)
                    {
                        // Delete related contracts first
                        if (client.Contrats != null)
                        {
                            _context.Contrats.RemoveRange(client.Contrats);
                        }
                        _context.Clients.Remove(client);
                    }
                }

                if (userRoles.Contains("Prestataire"))
                {
                    var prestataire = await _context.Prestataires
                        .Include(p => p.Prestations)
                        .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                    if (prestataire != null)
                    {
                        // Delete related prestations first
                        if (prestataire.Prestations != null)
                        {
                            _context.Prestations.RemoveRange(prestataire.Prestations);
                        }
                        _context.Prestataires.Remove(prestataire);
                    }
                }

                if (userRoles.Contains("Societe"))
                {
                    var societe = await _context.Societes
                        .Include(s => s.Services)
                        .Include(s => s.Prestations)
                        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
                    if (societe != null)
                    {
                        // Delete related services and prestations first
                        if (societe.Services != null)
                        {
                            _context.Services.RemoveRange(societe.Services);
                        }
                        if (societe.Prestations != null)
                        {
                            _context.Prestations.RemoveRange(societe.Prestations);
                        }
                        _context.Societes.Remove(societe);
                    }
                }

                await _context.SaveChangesAsync();

                // Delete the user
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> ApproveCompanyAsync(int societeId)
        {
            var societe = await _context.Societes.FindAsync(societeId);
            if (societe == null) return false;
            
            societe.IsApproved = true;
            societe.ApprovedDate = DateTime.Now;
            var currentUser = _httpContextAccessor.HttpContext?.User;
            societe.ApprovedBy = currentUser != null ? _userManager.GetUserId(currentUser) ?? "System" : "System";
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectCompanyAsync(int societeId)
        {
            var societe = await _context.Societes.FindAsync(societeId);
            if (societe == null) return false;
            
            // Optionally deactivate the associated user account
            var user = await _userManager.FindByIdAsync(societe.ApplicationUserId);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApprovePrestataireAsync(int prestataireId)
        {
            var prestataire = await _context.Prestataires.FindAsync(prestataireId);
            if (prestataire == null) return false;
            
            prestataire.IsApproved = true;
            prestataire.ApprovedDate = DateTime.Now;
            var currentUser = _httpContextAccessor.HttpContext?.User;
            prestataire.ApprovedBy = currentUser != null ? _userManager.GetUserId(currentUser) ?? "System" : "System";
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPrestataireAsync(int prestataireId)
        {
            var prestataire = await _context.Prestataires.FindAsync(prestataireId);
            if (prestataire == null) return false;
            
            // Optionally deactivate the associated user account
            var user = await _userManager.FindByIdAsync(prestataire.ApplicationUserId);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Societe>> GetPendingCompaniesAsync()
        {
            return await _context.Societes
                .Where(s => !s.IsApproved)
                .ToListAsync();
        }

        public async Task<List<Prestataire>> GetPendingPrestatairesAsync()
        {
            return await _context.Prestataires
                .Where(p => !p.IsApproved)
                .ToListAsync();
        }
    }
}
