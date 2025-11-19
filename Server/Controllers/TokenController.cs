using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DataContext;
using Server.Repositories;
using Shared.Dtos.Registrations;
using Shared.Dtos.Responses;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class TokenController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly ITokenService _service;
        public TokenController(AppDbContext ctx, ITokenService service)
        {
            this._ctx = ctx;
            this._service = service;

        }

        [HttpPost("Refresh")]
        [AllowAnonymous]
        public IActionResult Refresh(RefreshTokenRequest tokenApiModel)
        {
            //if (tokenApiModel is null)
            //    return BadRequest("Invalid client request");
            //string accessToken = tokenApiModel.AccessToken;
            //string refreshToken = tokenApiModel.RefreshToken;
            //var principal = _service.GetPrincipalFromExpiredToken(accessToken);
            //var username = principal.Identity!.Name;
            //var user = _ctx.TokenInfos.SingleOrDefault(u => u.UserName == username);
            //if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            //    return BadRequest("Invalid client request");
            //var newAccessToken = _service.GetToken(principal.Claims);
            //var newRefreshToken = _service.GetRefreshToken();
            //user.RefreshToken = newRefreshToken;
            //_ctx.SaveChanges();
            //return Ok(new LoginResponse
            //{
            //    Name = username!,
            //    UserName = user.UserName!,
            //    Token = newAccessToken.TokenString,
            //    RefreshToken = newRefreshToken,
            //    Expiration = newAccessToken.ValidTo,
            //    Code = 1,
            //    Message = "Token Refreshed"
            //});
            return Ok();
        }

        //revoken is use for removing token enntry
        [HttpPost("Revoke"), Authorize]
        public IActionResult Revoke()
        {
            try
            {
                var username = User.Identity!.Name;
                var user = User;
                if (user is null)
                    return BadRequest("User not found");
                //user.RefreshToken = null!;
                _ctx.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return BadRequest(message);
            }
        }
    }
}
