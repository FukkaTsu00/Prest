using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionPrestation.Models;
using GestionPrestation.Services;
using GestionPrestation.Models.ViewModels;

namespace GestionPrestation.Controllers
{
    [Authorize(Policy = "RequirePrestataire")]
    public class PrestataireController : Controller
    {
        private readonly IPrestataireService _prestataireService;
        private readonly IPrestationService _prestationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PrestataireController> _logger;

        public PrestataireController(
            IPrestataireService prestataireService,
            IPrestationService prestationService,
            UserManager<ApplicationUser> userManager,
            ILogger<PrestataireController> logger)
        {
            _prestataireService = prestataireService;
            _prestationService = prestationService;
            _userManager = userManager;
            _logger = logger;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound("Prestataire profile not found.");

            var assignedPrestations = await _prestationService.GetAssignedPrestationsAsync(prestataire.Id);
            var activePrestations = assignedPrestations.Where(p => p.Statut == PrestationStatus.EnCours || p.Statut == PrestationStatus.Assignee).ToList();
            var availablePrestations = await _prestationService.GetAvailablePrestationsAsync(prestataire.Id);

            var model = new PrestataireDashboardViewModel
            {
                Prestataire = prestataire,
                ActivePrestations = activePrestations,
                TotalRevenue = await _prestataireService.GetTotalEarningsAsync(prestataire.Id),
                PendingServices = availablePrestations.Count,
                CompletedServices = await _prestataireService.GetCompletedPrestationsCountAsync(prestataire.Id)
            };

            return View(model);
        }

        // Profile Management
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            return View(prestataire);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Prestataire updatedPrestataire)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var result = await _prestataireService.UpdatePrestataireAsync(updatedPrestataire, user.Id);
                if (result)
                {
                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError("", "Failed to update profile.");
            }
            return View(updatedPrestataire);
        }

        // Availability Management
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvailability(bool disponible)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var result = await _prestataireService.UpdateAvailabilityAsync(prestataire.Id, disponible);
            if (result)
                TempData["Success"] = $"Availability updated to {(disponible ? "Available" : "Unavailable")}.";
            else
                TempData["Error"] = "Failed to update availability.";

            return RedirectToAction(nameof(Profile));
        }

        // View Assigned Prestations
        public async Task<IActionResult> MyPrestations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var prestations = await _prestationService.GetAssignedPrestationsAsync(prestataire.Id);
            return View(prestations);
        }

        // View Available Prestations
        public async Task<IActionResult> AvailablePrestations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var prestations = await _prestationService.GetAvailablePrestationsAsync(prestataire.Id);
            return View(prestations);
        }

        // View Prestation Details
        [HttpGet]
        public async Task<IActionResult> PrestationDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var prestation = await _prestationService.GetPrestationByIdAsync(id);
            if (prestation == null || prestation.IdPrestataire != prestataire.Id)
                return Forbid();

            return View(prestation);
        }

        // Accept Prestation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptPrestation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var result = await _prestationService.AcceptPrestationAsync(id, prestataire.Id);
            if (result)
                TempData["Success"] = "Prestation accepted successfully.";
            else
                TempData["Error"] = "Failed to accept prestation.";

            return RedirectToAction(nameof(MyPrestations));
        }

        // Refuse Prestation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefusePrestation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var result = await _prestationService.RefusePrestationAsync(id, prestataire.Id);
            if (result)
                TempData["Success"] = "Prestation refused successfully.";
            else
                TempData["Error"] = "Failed to refuse prestation.";

            return RedirectToAction(nameof(AvailablePrestations));
        }

        // Update Prestation Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? notes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var result = await _prestationService.UpdatePrestationStatusAsync(id, prestataire.Id, status, notes);
            if (result)
                TempData["Success"] = "Status updated successfully.";
            else
                TempData["Error"] = "Failed to update status. Invalid status transition.";

            return RedirectToAction(nameof(PrestationDetails), new { id });
        }

        // Update Progress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int id, string progressUpdate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            if (string.IsNullOrWhiteSpace(progressUpdate))
            {
                TempData["Error"] = "Progress update cannot be empty.";
                return RedirectToAction(nameof(PrestationDetails), new { id });
            }

            var result = await _prestationService.UpdateProgressAsync(id, prestataire.Id, progressUpdate);
            if (result)
                TempData["Success"] = "Progress updated successfully.";
            else
                TempData["Error"] = "Failed to update progress.";

            return RedirectToAction(nameof(PrestationDetails), new { id });
        }

        // Performance Metrics
        public async Task<IActionResult> Performance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            var metrics = await _prestataireService.GetPerformanceMetricsAsync(prestataire.Id);
            var totalEarnings = await _prestataireService.GetTotalEarningsAsync(prestataire.Id);
            var completedCount = await _prestataireService.GetCompletedPrestationsCountAsync(prestataire.Id);
            var avgRating = await _prestataireService.GetAverageRatingAsync(prestataire.Id);

            ViewBag.Metrics = metrics;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.CompletedCount = completedCount;
            ViewBag.AverageRating = avgRating;

            return View();
        }

        // Respond to Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToReview(int id, string response)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var prestataire = await _prestataireService.GetPrestataireByUserIdAsync(user.Id);
            if (prestataire == null) return NotFound();

            if (string.IsNullOrWhiteSpace(response))
            {
                TempData["Error"] = "Response cannot be empty.";
                return RedirectToAction(nameof(PrestationDetails), new { id });
            }

            var result = await _prestataireService.RespondToReviewAsync(id, prestataire.Id, response);
            if (result)
                TempData["Success"] = "Response submitted successfully.";
            else
                TempData["Error"] = "Failed to submit response.";

            return RedirectToAction(nameof(PrestationDetails), new { id });
        }
    }
}

