// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GestionPrestation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using GestionPrestation.Data;

namespace GestionPrestation.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Account Type")]
            public string RoleName { get; set; } = "Client";

            [Required]
            [StringLength(20)]
            [Display(Name = "Phone Number")]
            public string Telephone { get; set; }

            [StringLength(100)]
            [Display(Name = "Specialty")]
            public string Specialite { get; set; }

            [StringLength(100)]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }

            [StringLength(255)]
            [Display(Name = "Company Address")]
            public string CompanyAddress { get; set; }

            [StringLength(50)]
            [Display(Name = "Company Registration Number")]
            public string CompanyRegistrationNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null, string role = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!string.IsNullOrEmpty(role))
            {
                Input.RoleName = role;
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var role = Input.RoleName ?? "Client";
                    var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!addRoleResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, "Impossible d'assigner le rôle.");
                        return Page();
                    }

                    if (role == "Client")
                    {
                        var client = new Models.Client
                        {
                            ApplicationUserId = user.Id,
                            Nom = Input.LastName,
                            Prenom = Input.FirstName,
                            Telephone = Input.Telephone,
                            Email = Input.Email,
                            Adresse = string.Empty,
                            TypeClient = "Particulier"
                        };
                        _context.Clients.Add(client);
                    }
                    else if (role == "Prestataire")
                    {
                        if (string.IsNullOrWhiteSpace(Input.Specialite))
                        {
                            await _userManager.DeleteAsync(user);
                            ModelState.AddModelError(string.Empty, "Prestataires must specify a specialty.");
                            return Page();
                        }

                        var prestataire = new Models.Prestataire
                        {
                            ApplicationUserId = user.Id,
                            Nom = Input.LastName,
                            Prenom = Input.FirstName,
                            Telephone = Input.Telephone,
                            Specialite = Input.Specialite,
                        };
                        _context.Prestataires.Add(prestataire);
                    }
                    else if (role == "Societe")
                    {
                        if (string.IsNullOrWhiteSpace(Input.CompanyName) || string.IsNullOrWhiteSpace(Input.CompanyAddress) || string.IsNullOrWhiteSpace(Input.CompanyRegistrationNumber))
                        {
                            await _userManager.DeleteAsync(user);
                            ModelState.AddModelError(string.Empty, "Company name, address, and registration number are required.");
                            return Page();
                        }

                        var societe = new Models.Societe
                        {
                            ApplicationUserId = user.Id,
                            Nom = Input.CompanyName,
                            Adresse = Input.CompanyAddress,
                            Email = Input.Email,
                            NumeroStringCommerce = Input.CompanyRegistrationNumber,
                        };
                        _context.Societes.Add(societe);
                    }

                    await _context.SaveChangesAsync();

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        // Redirect to role-specific dashboard after registration
                        if (role == "Admin")
                        {
                            return Redirect("/Admin/Dashboard");
                        }
                        else if (role == "Client")
                        {
                            return Redirect("/Client/Dashboard");
                        }
                        else if (role == "Prestataire")
                        {
                            return Redirect("/Prestataire/Dashboard");
                        }
                        else if (role == "Societe")
                        {
                            return Redirect("/Company/Dashboard");
                        }
                        return LocalRedirect(returnUrl ?? Url.Content("~/"));
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}