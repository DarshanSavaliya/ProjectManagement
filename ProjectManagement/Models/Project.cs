using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models {
    public class Project {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<TaskAssigned> Tasks { get; set; } = new List<TaskAssigned>();
        [ForeignKey("ApplicationUser")]
        public string Admin { get; set; } = "";
        public List<UserProject> Members { get; set; } = new List<UserProject>();
    }
}
