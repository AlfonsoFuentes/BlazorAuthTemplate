using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using Toolbelt.Blazor;

namespace CllientMudBlazor.Services
{
    public class HttpInterceptorService : IDisposable
    {
        private readonly IHttpClientInterceptor _interceptor;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigation;
        private bool _isRedirecting;

        public HttpInterceptorService(
            IHttpClientInterceptor interceptor,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            NavigationManager navigation)
        {
            _interceptor = interceptor;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _navigation = navigation;

            _interceptor.AfterSendAsync += InterceptResponseAsync;
        }

        private async Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            if (e.Response?.StatusCode == HttpStatusCode.Unauthorized && !_isRedirecting)
            {
                _isRedirecting = true;

                await _localStorage.RemoveItemAsync("accessToken");
                ((AuthProvider)_authStateProvider).NotifyUserLogout();

                // Evita redirigir si ya estamos en login
                if (!_navigation.Uri.Contains("/login"))
                {
                    _navigation.NavigateTo("/login", forceLoad: true);
                }
            }
        }

        public void Dispose()
        {
            _interceptor.AfterSendAsync -= InterceptResponseAsync;
        }
    }
}
