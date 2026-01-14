using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestionPrestation.Models;
using GestionPrestation.Services;
using GestionPrestation.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestation.Controllers
{
    [Authorize(Policy = "RequireCompany")]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IPrestationService _prestationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ICompanyService companyService,
            IPrestationService prestationService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<CompanyController> logger)
        {
            _companyService = companyService;
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

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound("Company profile not found.");

            var model = await _companyService.GetCompanyDashboardAsync(company.Id);
            return View(model);
        }

        // Profile Management
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Societe updatedCompany)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var result = await _companyService.UpdateCompanyAsync(updatedCompany, user.Id);
                if (result)
                {
                    TempData["Success"] = "Company profile updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                ModelState.AddModelError("", "Failed to update company profile.");
            }
            return View(updatedCompany);
        }

        // Service Management
        public async Task<IActionResult> Services()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var services = await _companyService.GetCompanyServicesAsync(company.Id);
            return View(services);
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            ViewBag.Categories = new List<string> 
            { 
                Service.ServiceCategories.Consulting,
                Service.ServiceCategories.Development,
                Service.ServiceCategories.Design,
                Service.ServiceCategories.Marketing,
                Service.ServiceCategories.Support,
                Service.ServiceCategories.Training,
                Service.ServiceCategories.Maintenance,
                Service.ServiceCategories.Other
            };
            ViewBag.BillingTypes = new List<string>
            {
                Service.BillingTypes.Fixed,
                Service.BillingTypes.Hourly,
                Service.BillingTypes.Milestone,
                Service.BillingTypes.Subscription
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(Service service)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _companyService.CreateServiceAsync(service, company.Id);
                if (result != null)
                {
                    TempData["Success"] = "Service created successfully.";
                    return RedirectToAction(nameof(Services));
                }
                ModelState.AddModelError("", "Failed to create service.");
            }

            ViewBag.Categories = new List<string> 
            { 
                Service.ServiceCategories.Consulting,
                Service.ServiceCategories.Development,
                Service.ServiceCategories.Design,
                Service.ServiceCategories.Marketing,
                Service.ServiceCategories.Support,
                Service.ServiceCategories.Training,
                Service.ServiceCategories.Maintenance,
                Service.ServiceCategories.Other
            };
            ViewBag.BillingTypes = new List<string>
            {
                Service.BillingTypes.Fixed,
                Service.BillingTypes.Hourly,
                Service.BillingTypes.Milestone,
                Service.BillingTypes.Subscription
            };
            return View(service);
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null || service.IdSociete != company.Id)
                return Forbid();

            ViewBag.Categories = new List<string> 
            { 
                Service.ServiceCategories.Consulting,
                Service.ServiceCategories.Development,
                Service.ServiceCategories.Design,
                Service.ServiceCategories.Marketing,
                Service.ServiceCategories.Support,
                Service.ServiceCategories.Training,
                Service.ServiceCategories.Maintenance,
                Service.ServiceCategories.Other
            };
            ViewBag.BillingTypes = new List<string>
            {
                Service.BillingTypes.Fixed,
                Service.BillingTypes.Hourly,
                Service.BillingTypes.Milestone,
                Service.BillingTypes.Subscription
            };
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, Service service)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _companyService.UpdateServiceAsync(service, company.Id);
                if (result)
                {
                    TempData["Success"] = "Service updated successfully.";
                    return RedirectToAction(nameof(Services));
                }
                ModelState.AddModelError("", "Failed to update service.");
            }

            ViewBag.Categories = new List<string> 
            { 
                Service.ServiceCategories.Consulting,
                Service.ServiceCategories.Development,
                Service.ServiceCategories.Design,
                Service.ServiceCategories.Marketing,
                Service.ServiceCategories.Support,
                Service.ServiceCategories.Training,
                Service.ServiceCategories.Maintenance,
                Service.ServiceCategories.Other
            };
            ViewBag.BillingTypes = new List<string>
            {
                Service.BillingTypes.Fixed,
                Service.BillingTypes.Hourly,
                Service.BillingTypes.Milestone,
                Service.BillingTypes.Subscription
            };
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var result = await _companyService.DeleteServiceAsync(id, company.Id);
            if (result)
                TempData["Success"] = "Service deleted successfully.";
            else
                TempData["Error"] = "Failed to delete service.";

            return RedirectToAction(nameof(Services));
        }

        // Prestataire Management
        public async Task<IActionResult> Prestataires()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var prestataires = await _companyService.GetCompanyPrestatairesAsync(company.Id);
            return View(prestataires);
        }

        [HttpGet]
        public IActionResult RegisterPrestataire()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPrestataire(Prestataire prestataire)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Note: In a real scenario, you'd create a user account first
                // For now, assuming prestataire is linked to company
                var result = await _companyService.RegisterPrestataireAsync(prestataire, company.Id, user.Id);
                if (result)
                {
                    TempData["Success"] = "Prestataire registered successfully.";
                    return RedirectToAction(nameof(Prestataires));
                }
                ModelState.AddModelError("", "Failed to register prestataire.");
            }
            return View(prestataire);
        }

        // Prestation Management
        public async Task<IActionResult> Prestations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var prestations = await _companyService.GetCompanyPerformanceAsync(company.Id);
            return View(prestations);
        }

        [HttpGet]
        public async Task<IActionResult> PrestationDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var prestation = await _prestationService.GetPrestationByIdAsync(id);
            if (prestation?.Service?.IdSociete != company.Id)
                return Forbid();

            var prestataires = await _companyService.GetCompanyPrestatairesAsync(company.Id);
            ViewBag.Prestataires = prestataires;
            return View(prestation);
        }

        // Assign Prestation to Prestataire
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPrestation(int prestationId, int prestataireId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var result = await _prestationService.AssignPrestationToPrestataireAsync(prestationId, prestataireId, company.Id);
            if (result)
                TempData["Success"] = "Prestation assigned successfully.";
            else
                TempData["Error"] = "Failed to assign prestation.";

            return RedirectToAction(nameof(PrestationDetails), new { id = prestationId });
        }

        // Update Pricing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePricing(int prestationId, decimal prixFinal)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            if (prixFinal <= 0)
            {
                TempData["Error"] = "Price must be greater than zero.";
                return RedirectToAction(nameof(PrestationDetails), new { id = prestationId });
            }

            var result = await _prestationService.UpdatePrestationPricingAsync(prestationId, prixFinal, company.Id);
            if (result)
                TempData["Success"] = "Pricing updated successfully.";
            else
                TempData["Error"] = "Failed to update pricing.";

            return RedirectToAction(nameof(PrestationDetails), new { id = prestationId });
        }

        // Performance Dashboard
        public async Task<IActionResult> Performance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var company = await _companyService.GetCompanyByUserIdAsync(user.Id);
            if (company == null) return NotFound();

            var prestations = await _companyService.GetCompanyPerformanceAsync(company.Id);
            return View(prestations);
        }
    }
}

