using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using ProjectManagement.Models.DTO;
using ProjectManagement.Models.Helper;

namespace ProjectManagement.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController: ControllerBase {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public TaskController(DataContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            this.userManager = userManager;
        }

        // GET: api/Task
        // Retrieve all the tasks of projects the user is member or admin of.
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskAssigned>>> GetAllTasks() {
            var name = HttpContext.User?.Identity?.Name;

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null) return Forbid();

            var projects = await _context.Projects.ToListAsync();

            var roles = await userManager.GetRolesAsync(user);
            if(!roles.Contains(UserRoles.Admin)) {
                foreach(var project in projects) {
                    await _context.Entry(project).Collection(b => b.Members).LoadAsync();
                }
                projects.RemoveAll(project => project.Admin != user.UserName && !project.Members.Exists(member => member.Member == user.UserName));


            }

            List<TaskAssigned> tasks = new();
            foreach(var project in projects) {
                await _context.Entry(project).Collection(b => b.Tasks).LoadAsync();
                foreach(var task in project.Tasks) tasks.Add(task);
            }

            return tasks;
        }

        // GET: api/Task/5
        // Retrieve task info if you have access to this task
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<TaskAssigned>> GetTask(int id) {
            var name = HttpContext.User?.Identity?.Name;

            var task = await _context.Tasks.FindAsync(id);
            if(task == null) return NotFound();

            var project = await _context.Projects.FirstOrDefaultAsync(op => op.Id == task.ProjectId);
            if(project == null) return NotFound();

            await _context.Entry(project).Collection(b => b.Members).LoadAsync();
            if(project.Admin != name && !project.Members.Exists(b => b.Member == name)) return Forbid(); 

            return Ok(task);
        }

        // POST: api/Task
        // Create a new task if user is admin of the project
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TaskAssigned>> CreateTask(TaskDTO taskDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var project = await _context.Projects.FindAsync(taskDTO.ProjectId);
            if(project == null) return NotFound(taskDTO.ProjectId);

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.Email) return Forbid();

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

        // PUT: api/Task
        // Update task if user is admin of project
        [HttpPut]
        public async Task<ActionResult<TaskAssigned>> UpdateTask(TaskDTO taskDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var project = await _context.Projects.FindAsync(taskDTO.ProjectId);
            if(project == null) return NotFound(taskDTO.ProjectId);

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.Email) return Forbid();

            var task = await _context.Tasks.FindAsync(taskDTO.Id);
            if(task == null || task.ProjectId != project.Id) return NotFound();

            task.Name = taskDTO.Name;
            task.StartDate = taskDTO.StartDate;
            task.EndDate = taskDTO.EndDate;
            task.WorkHours = taskDTO.WorkHours;
            task.Reminder = taskDTO.Reminder;
            task.Attachment = taskDTO.Attachment;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // DELETE: api/Task/5
        // Delete the task if user is admin
        [HttpDelete("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TaskAssigned>> DeleteTask(int id) {
            var name = HttpContext.User?.Identity?.Name;

            var task = await _context.Tasks.FindAsync(id);
            if(task == null) return NotFound();

            var project = await _context.Projects.FindAsync(task.ProjectId);
            if(project == null) return NotFound(task.ProjectId);

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.Email) return Forbid();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // POST: api/Task/Assign/5/email@email.com
        // Assign a task to member of project
        [HttpPost("Assign/{id:int}/{username}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TaskAssigned>> AssignTask(int id, string username) {
            var name = HttpContext.User?.Identity?.Name;

            var task = await _context.Tasks.FindAsync(id);
            if(task == null) return NotFound();

            var project = await _context.Projects.FindAsync(task.ProjectId);
            if(project == null) return NotFound(task.ProjectId);

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.Email) return Forbid();

            await _context.Entry(project).Collection(b => b.Members).LoadAsync();
            if(project.Admin != username && project.Members.Exists(b => b.Member == username)) return BadRequest("User is not a member of Project");

            task.Assignee = await _context.Users.FirstOrDefaultAsync(op => op.UserName == username);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // PUT: api/Assign/5/email@email.com
        // Assign a task to some other member
        [HttpPut("Assign/{id:int}/{username}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<TaskAssigned>> UpdateAssignee(int id, string username) {
            var name = HttpContext.User?.Identity?.Name;

            var task = await _context.Tasks.FindAsync(id);
            if(task == null) return NotFound();

            if(task.Assignee?.UserName == username) return Ok("User has already been assigned the task");

            var project = await _context.Projects.FindAsync(task.ProjectId);
            if(project == null) return NotFound(task.ProjectId);

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.Email) return Forbid();

            await _context.Entry(project).Collection(b => b.Members).LoadAsync();
            if(project.Admin != username && project.Members.Exists(b => b.Member == username)) return BadRequest("User is not a member of Project");

            task.Assignee = await _context.Users.FirstOrDefaultAsync(op => op.UserName == username);
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}
