using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models {
    public class UserProject {
        [Key]
        public int Id { get; set; }
        public IdentityUser User { get; set; } = new IdentityUser();
        public Project Project { get; set; } = new Project();
    }
}
