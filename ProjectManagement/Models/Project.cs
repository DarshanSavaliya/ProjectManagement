using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models {
    public class Project {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<TaskAssigned> Tasks { get; set; } = new List<TaskAssigned>();
        public IdentityUser Admin { get; set; } = new IdentityUser();
    }
}
