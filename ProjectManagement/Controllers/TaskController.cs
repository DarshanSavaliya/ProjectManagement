using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using ProjectManagement.Models.DTO;

namespace ProjectManagement.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController: ControllerBase {
        private readonly DataContext _context;

        public TaskController(DataContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAssigned>>> GetAllTasks() {
            return await _context.Tasks.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskAssigned>> GetTask(int id) {
            var task = await _context.Tasks.FindAsync(id);

            if(task == null) return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskAssigned>> CreateTask(TaskDTO taskDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var project = await _context.Projects.FindAsync(taskDTO.ProjectId);

            if(project == null) return NotFound(taskDTO.ProjectId);

            TaskAssigned task = new() {
                Name = taskDTO.Name,
                StartDate = taskDTO.StartDate,
                EndDate = taskDTO.EndDate,
                WorkHours = taskDTO.WorkHours,
                Reminder = taskDTO.Reminder,
                Attachment = taskDTO.Attachment
            };

            project.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpPut]
        public async Task<ActionResult<TaskAssigned>> UpdateTask(TaskDTO taskDto) {
            if(!ModelState.IsValid) return BadRequest();

            var task = await _context.Tasks.FindAsync(taskDto.Id);

            if(task == null) return NotFound();

            task.Name = taskDto.Name;
            task.StartDate = taskDto.StartDate;
            task.EndDate = taskDto.EndDate;
            task.WorkHours = taskDto.WorkHours;
            task.Reminder = taskDto.Reminder;
            task.Attachment = taskDto.Attachment;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TaskAssigned>> DeleteTask(int id) {
            if(!ModelState.IsValid) return BadRequest();

            var task = await _context.Tasks.FindAsync(id);

            if(task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}
