using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models {
    public class TaskAssigned {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int WorkHours { get; set; } = 0;
        public DateTime? Reminder { get; set; }
        public string? Attachment { get; set; }
        public Project Project { get; set; } = new Project();
        public IdentityUser? Assignee { get; set; }
    }
}
