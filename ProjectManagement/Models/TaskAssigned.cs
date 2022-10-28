using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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
        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public ApplicationUser? Assignee { get; set; } 
    }
}
