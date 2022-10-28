using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models {
    public class UserProject {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string Member { get; set; }

        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
    }
}
