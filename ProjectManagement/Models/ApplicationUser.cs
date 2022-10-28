using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models {
    public class ApplicationUser: IdentityUser {
        [Key]
        public override string Email { get; set; } = "";
    }
}
