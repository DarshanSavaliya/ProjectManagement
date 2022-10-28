using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;
using ProjectManagement.Models.DTO;
using ProjectManagement.Models.Helper;

namespace ProjectManagement.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController: ControllerBase {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public ProjectController(DataContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            this.userManager = userManager;
            this.userManager = userManager;
        }

        // GET: api/Project
        // All projects can be retrieved in which user is an admin or a member
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects() {
            var name = HttpContext.User?.Identity?.Name;

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null) return Forbid();

            var projects = await _context.Projects.ToListAsync();

            var roles = await userManager.GetRolesAsync(user);
            if(roles.Contains(UserRoles.Admin)) return projects;

            foreach(var project in projects) {
                await _context.Entry(project).Collection(b => b.Members).LoadAsync();
            }

            projects.RemoveAll(project => project.Admin != user.UserName && !project.Members.Exists(member => member.Member == user.UserName));

            return projects;
        }

        // GET: api/Project/5
        // Project can be accessed only if user is admin or member
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Project>> GetProject(int id) {
            var name =  HttpContext.User?.Identity?.Name;

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null) return Forbid();

            var project = await _context.Projects.FindAsync(id);
            if(project == null) return NotFound();

            await _context.Entry(project).Collection(b => b.Tasks).LoadAsync();
            await _context.Entry(project).Collection(b => b.Members).LoadAsync();

            var isMember = project.Members.Find(member => member.Member == user.UserName);
            if(isMember == null && project.Admin != user.UserName) return Forbid();


            return Ok(project);
        }

        // POST: api/Project
        // Only Admins can create a project
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Project>> CreateProject(ProjectDTO projectDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null) return Forbid();

            Project project = new() {
                Name = projectDTO.Name,
                Description = projectDTO.Description ?? "",
                Admin = user.UserName,
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // PUT: api/Project
        // Only Admins can update the project details
        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Project>> UpdateProject(ProjectDTO projectDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var project = await _context.Projects.FindAsync(projectDTO.Id);
            if(project == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.UserName) return Forbid();

            project.Name = projectDTO.Name;
            project.Description = projectDTO.Description ?? "";

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // DELETE: api/Project/5
        // Only Admins can delete project
        [HttpDelete("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> DeleteProject(int id) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var project = await _context.Projects.FindAsync(id);
            if(project == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.UserName) return Forbid();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // POST api/Project/AddMember/5/user@email.com
        // Only admins can add members to the project
        [HttpPost("addMember/{id:int}/{username}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> AddMember(int id, string username) {
            if(!ModelState.IsValid) return BadRequest();

            var name = HttpContext.User?.Identity?.Name;

            var project = await _context.Projects.FindAsync(id);
            if(project == null) return NotFound("Project Not Found");

            var user = await _context.Users.FirstOrDefaultAsync(op => op.UserName == name);
            if(user == null || project.Admin != user.UserName) return Forbid();

            await _context.Entry(project).Collection(b => b.Members).LoadAsync();
            if(project.Admin == username || project.Members.Exists(member => member.Member == username)) return StatusCode(StatusCodes.Status500InternalServerError, "User is already a member of this Porject");

            project.Members.Add(new UserProject() {
                Member = username,
                ProjectId = project.Id
            });

            await _context.SaveChangesAsync();

            return Ok(project);
        }
    }
}
