using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CllientMudBlazor.Services
{
    public interface IUserInfoService
    {
        Task<ClaimsPrincipal> GetUserNameAsync(); // ✅ async
    }
    public class UserInfoService : IUserInfoService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public UserInfoService(AuthenticationStateProvider authStateProvider)
            => _authStateProvider = authStateProvider;

        public async Task<ClaimsPrincipal> GetUserNameAsync() // ✅ async Task<string>
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync(); // ✅ await
            var user = authState.User;
            var claims = user.Claims.ToList();
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var result = name
               ?? email
               ?? "Guest";
            return user;
        }
    }
}
