using GestionPrestation.Models;

namespace GestionPrestation.Models.ViewModels
{
    public class UserWithRoles
    {
        public string Id { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
        
        public ApplicationUser User { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public Client? Client { get; set; }
        public Prestataire? Prestataire { get; set; }
        public Societe? Societe { get; set; }
    }
}
