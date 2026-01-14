using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionPrestation.Models;
using GestionPrestation.Services;
using GestionPrestation.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestation.Controllers
{
    [Authorize(Policy = "RequireClient")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IPrestationService _prestationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            IPrestationService prestationService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _prestationService = prestationService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound("Client profile not found.");

            var activePrestations = await _clientService.GetClientActivePrestationsAsync(client.Id);
            var history = await _clientService.GetClientPrestationHistoryAsync(client.Id);

            ViewBag.Client = client;
            ViewBag.ActivePrestations = activePrestations;
            ViewBag.History = history.Take(10).ToList();
            ViewBag.TotalPrestations = history.Count;
            ViewBag.CompletedPrestations = history.Count(p => p.Statut == PrestationStatus.Validee || p.Statut == PrestationStatus.Terminee);

            return View();
        }

        // Profile Management
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Client updatedClient)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var result = await _clientService.UpdateClientAsync(updatedClient, user.Id);
                if (result)
                {
                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError("", "Failed to update profile.");
            }
            return View(updatedClient);
        }

        // Prestation Requests
        public async Task<IActionResult> MyPrestations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            var prestations = await _clientService.GetClientPrestationHistoryAsync(client.Id);
            return View(prestations);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRequest()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .Include(s => s.Societe)
                .ToListAsync();
            
            ViewBag.Services = services;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(int serviceId, string description)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            if (string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("", "Description is required.");
                var services = await _context.Services.Where(s => s.IsActive).ToListAsync();
                ViewBag.Services = services;
                return View();
            }

            var prestation = await _prestationService.CreatePrestationRequestAsync(serviceId, client.Id, description);
            if (prestation != null)
            {
                TempData["Success"] = "Prestation request created successfully.";
                return RedirectToAction(nameof(MyPrestations));
            }

            TempData["Error"] = "Failed to create prestation request.";
            var servicesList = await _context.Services.Where(s => s.IsActive).ToListAsync();
            ViewBag.Services = servicesList;
            return View();
        }

        // View Prestation Details
        [HttpGet]
        public async Task<IActionResult> PrestationDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            var prestation = await _prestationService.GetPrestationByIdAsync(id);
            if (prestation == null || prestation.IdClient != client.Id)
                return Forbid();

            return View(prestation);
        }

        // Rate and Review Prestation
        [HttpGet]
        public async Task<IActionResult> RatePrestation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            var prestation = await _prestationService.GetPrestationByIdAsync(id);
            if (prestation == null || prestation.IdClient != client.Id)
                return Forbid();

            if (prestation.Statut != PrestationStatus.Terminee && prestation.Statut != PrestationStatus.Validee)
            {
                TempData["Error"] = "You can only rate completed prestations.";
                return RedirectToAction(nameof(PrestationDetails), new { id });
            }

            return View(prestation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RatePrestation(int id, int rating, string? feedback)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("", "Rating must be between 1 and 5.");
                var prestation = await _prestationService.GetPrestationByIdAsync(id);
                return View(prestation);
            }

            var result = await _prestationService.RatePrestationAsync(id, rating, feedback, client.Id);
            if (result)
            {
                TempData["Success"] = "Thank you for your rating!";
                return RedirectToAction(nameof(PrestationDetails), new { id });
            }

            TempData["Error"] = "Failed to submit rating.";
            return RedirectToAction(nameof(RatePrestation), new { id });
        }

        // Cancel Prestation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelPrestation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var client = await _clientService.GetClientByUserIdAsync(user.Id);
            if (client == null) return NotFound();

            var result = await _prestationService.CancelPrestationAsync(id, client.Id);
            if (result)
                TempData["Success"] = "Prestation cancelled successfully.";
            else
                TempData["Error"] = "Failed to cancel prestation. It may not be cancellable.";

            return RedirectToAction(nameof(MyPrestations));
        }
    }
}
