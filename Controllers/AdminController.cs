using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionPrestation.Models;
using GestionPrestation.Models.ViewModels;
using GestionPrestation.Services;

namespace GestionPrestation.Controllers
{
    [Authorize(Policy = "RequireAdmin")]
    public class AdminController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IPrestationService _prestationService;
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IUserManagementService userManagementService,
            IPrestationService prestationService,
            ILogger<AdminController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _userManagementService = userManagementService;
            _prestationService = prestationService;
            _logger = logger;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                var allPrestations = await _prestationService.GetAllPrestationsAsync();
                
                var model = new AdminDashboardViewModel
                {
                    TotalClients = users?.Count(u => u.Roles.Contains("Client")) ?? 0,
                    TotalPrestataires = users?.Count(u => u.Roles.Contains("Prestataire")) ?? 0,
                    TotalSocietes = users?.Count(u => u.Roles.Contains("Societe")) ?? 0,
                    TotalPrestations = allPrestations?.Count ?? 0,
                    ActivePrestations = allPrestations?.Count(p => p.Statut == PrestationStatus.EnCours || p.Statut == PrestationStatus.Assignee) ?? 0,
                    TotalRevenue = allPrestations?.Where(p => p.Statut == PrestationStatus.Validee || p.Statut == PrestationStatus.Terminee).Sum(p => p.PrixFinal) ?? 0,
                    RecentPrestations = allPrestations?.Take(10).ToList() ?? new List<Prestation>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return View(new AdminDashboardViewModel()); // Return empty model on error
            }
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return View(users ?? new List<UserWithRoles>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                return View(new List<UserWithRoles>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ApplicationUser user, string password, string role)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManagementService.CreateUserAsync(user, password, role);
                if (result)
                {
                    TempData["Success"] = "User created successfully.";
                    return RedirectToAction(nameof(Users));
                }
                ModelState.AddModelError("", "Failed to create user.");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user.User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, ApplicationUser updatedUser)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManagementService.UpdateUserAsync(id, updatedUser);
                if (result)
                {
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Users));
                }
                ModelState.AddModelError("", "Failed to update user.");
            }
            return View(updatedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var result = await _userManagementService.DeactivateUserAsync(id);
            if (result)
                TempData["Success"] = "User deactivated successfully.";
            else
                TempData["Error"] = "Failed to deactivate user.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var result = await _userManagementService.ActivateUserAsync(id);
            if (result)
                TempData["Success"] = "User activated successfully.";
            else
                TempData["Error"] = "Failed to activate user.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var result = await _userManagementService.AssignRoleAsync(userId, role);
            if (result)
                TempData["Success"] = $"Role '{role}' assigned successfully.";
            else
                TempData["Error"] = "Failed to assign role.";
            return RedirectToAction(nameof(UserDetails), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var result = await _userManagementService.RemoveRoleAsync(userId, role);
            if (result)
                TempData["Success"] = $"Role '{role}' removed successfully.";
            else
                TempData["Error"] = "Failed to remove role.";
            return RedirectToAction(nameof(UserDetails), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userManagementService.DeleteUserAsync(id);
            if (result)
                TempData["Success"] = "User deleted successfully.";
            else
                TempData["Error"] = "Failed to delete user.";
            return RedirectToAction(nameof(Users));
        }

        // Approval Management
        public async Task<IActionResult> PendingApprovals()
        {
            var companies = await _userManagementService.GetPendingCompaniesAsync();
            var prestataires = await _userManagementService.GetPendingPrestatairesAsync();
            
            ViewBag.Companies = companies;
            ViewBag.Prestataires = prestataires;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCompany(int id)
        {
            var result = await _userManagementService.ApproveCompanyAsync(id);
            if (result)
                TempData["Success"] = "Company approved successfully.";
            else
                TempData["Error"] = "Failed to approve company.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCompany(int id)
        {
            var result = await _userManagementService.RejectCompanyAsync(id);
            if (result)
                TempData["Success"] = "Company rejected successfully.";
            else
                TempData["Error"] = "Failed to reject company.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePrestataire(int id)
        {
            var result = await _userManagementService.ApprovePrestataireAsync(id);
            if (result)
                TempData["Success"] = "Prestataire approved successfully.";
            else
                TempData["Error"] = "Failed to approve prestataire.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPrestataire(int id)
        {
            var result = await _userManagementService.RejectPrestataireAsync(id);
            if (result)
                TempData["Success"] = "Prestataire rejected successfully.";
            else
                TempData["Error"] = "Failed to reject prestataire.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        
        // All Prestations View
        public async Task<IActionResult> AllPrestations()
        {
            var prestations = await _prestationService.GetAllPrestationsAsync();
            return View(prestations);
        }
    }
}
