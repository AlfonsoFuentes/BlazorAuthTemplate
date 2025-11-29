using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Dtos.Requests;
using Shared.Dtos.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CllientMudBlazor.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest model);
        Task Logout();
        Task<StatusResponse> Register(RegistrationRequest model);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
     

        public AuthenticationService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            this._authStateProvider = authStateProvider;
            this._localStorage = localStorage;
    
        }


        public async Task<LoginResponse> Login(LoginRequest model)
        {

            var loginResult = await _httpClient.PostAsJsonAsync($"api/authorization/login", model);
            if (loginResult.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || loginResult.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return new LoginResponse { Code = 0, Message = "Server error" };
            var loginResponseContent = await loginResult.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponseContent != null && loginResponseContent.Code != 0)
            {
                await _localStorage.SetItemAsync("accessToken", loginResponseContent.Token);
                ((AuthProvider)_authStateProvider).NotifyUserAuthentication(loginResponseContent.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponseContent.Token);
            }
            return loginResponseContent!;
        }
        public async Task<StatusResponse> Register(RegistrationRequest model)
        {

            var loginResult = await _httpClient.PostAsJsonAsync($"api/authorization/Register", model);
            if (loginResult.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || loginResult.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return new StatusResponse { Code = 0, Message = "Server error" };
            var loginResponseContent = await loginResult.Content.ReadFromJsonAsync<StatusResponse>();
            
            return loginResponseContent!;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("accessToken");
            ((AuthProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;

        }

    }
}
