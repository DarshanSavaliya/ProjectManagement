using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.DTO {
    public class UserDTO {
        public string? Id { get; set; }
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
