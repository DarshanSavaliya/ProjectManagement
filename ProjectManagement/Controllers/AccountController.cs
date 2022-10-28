using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Models.DTO;
using ProjectManagement.Models.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(UserDTO userDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var user = await userManager.FindByNameAsync(userDTO.Email);
            
            if(user == null || !await userManager.CheckPasswordAsync(user, userDTO.Password)) return Unauthorized();

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach(var userRole in userRoles) {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
            });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(UserDTO userDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var userExists = await userManager.FindByNameAsync(userDTO.Email);
            if(userExists != null) {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            }

            IdentityUser user = new() {
                UserName = userDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = userDTO.Email
            };

            var result = await userManager.CreateAsync(user, userDTO.Password);  
            
            if(result.Succeeded) {
                return Ok(new Response {
                    Status = "Successful",
                    Message = "User created successfully!",
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        }

        [HttpPost]
        [Route("Register-Admin")]
        public async Task<ActionResult> RegisterAdmin(UserDTO userDTO) {
            if(!ModelState.IsValid) return BadRequest();

            var userExists = await userManager.FindByNameAsync(userDTO.Email);
            if(userExists != null) {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            }

            IdentityUser user = new() {
                UserName = userDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = userDTO.Email
            };

            var result = await userManager.CreateAsync(user, userDTO.Password);

            if(result.Succeeded) {

                if(!await roleManager.RoleExistsAsync(UserRoles.Admin)) await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if(!await roleManager.RoleExistsAsync(UserRoles.User)) await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                if(await roleManager.RoleExistsAsync(UserRoles.Admin)) await userManager.AddToRoleAsync(user, UserRoles.Admin);
                if(await roleManager.RoleExistsAsync(UserRoles.User)) await userManager.AddToRoleAsync(user, UserRoles.User);

                return Ok(new Response {
                    Status = "Successful",
                    Message = "Admin created successfully!",
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims) {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                expires: DateTime.Now.AddDays(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
