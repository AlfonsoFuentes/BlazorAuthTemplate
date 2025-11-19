using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.DataContext;
using Server.Domain.Identities;
using Server.Repositories;
using Shared.Commons;
using Shared.Dtos;
using Shared.Dtos.Registrations;
using Shared.Dtos.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorizationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService _tokenService;
        public AuthorizationController(AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._tokenService = tokenService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
        {
            var status = new StatusResponse();
            // check validations
            if (!ModelState.IsValid)
            {
                status.Code = 0;
                status.Message = "please pass all the valid fields";
                return BadRequest(status);
            }
            // lets find the user
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                status.Code = 0;
                status.Message = "invalid username";
                return NotFound(status);
            }
            // check current password
            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.Code = 0;
                status.Message = "invalid current password";
                return BadRequest(status);
            }

            // change password here
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.Code = 0;
                status.Message = "Failed to change password";
                return BadRequest(status);
            }
            status.Code = 1;
            status.Message = "Password has changed successfully";
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return NotFound(new StatusResponse() { Code = 0, Message = "User do not exist" });

            }
            if(await userManager.CheckPasswordAsync(user, model.Password)==false)
            {
                return BadRequest(new StatusResponse() { Code = 0, Message = "Invalid Password" });
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier,user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = _tokenService.GetToken(authClaims);
            var refreshToken = _tokenService.GetRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new LoginResponse
            {
                //Name = user.Name!,
                UserName = user.UserName!,
                Token = token.TokenString,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                Code = 1,
                Message = "Logged in"
            });
            //login failed condition

            
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest model)
        {
            var status = new StatusResponse();
            if (!ModelState.IsValid)
            {
                status.Code = 0;
                status.Message = "Please pass all the required fields";
                return BadRequest(status);
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.Code = 0;
                status.Message = "Invalid username";
                return BadRequest(status);
            }
            var userEmailExists = await userManager.FindByEmailAsync(model.Email);
            if (userEmailExists != null)
            {
                status.Code = 0;
                status.Message = "Email exist";
                return BadRequest(status);
            }
            var user = new AppUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                //Name = model.UserName
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.Code = 0;
                status.Messages = result.Errors.Select(x => x.Description).ToList();
                return BadRequest(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole() { Name = UserRoles.User });
            await userManager.AddToRoleAsync(user, UserRoles.User);

            status.Code = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }
    }
}
