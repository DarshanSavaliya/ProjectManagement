namespace ProjectManagement.Models.DTO {
    public class TaskDTO {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WorkHours { get; set; }
        public DateTime? Reminder { get; set; }
        public string? Attachment { get; set; }
        public int ProjectId { get; set; }
    }
}
