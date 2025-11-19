using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace CllientMudBlazor.Services
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService localStorage;

        public AuthenticationHeaderHandler(ILocalStorageService localStorage)
            => this.localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null || request.Headers.Authorization?.Scheme != "bearer")
            {
                var savedToken = await localStorage.GetItemAsync<string>("accessToken");

                if (!string.IsNullOrWhiteSpace(savedToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
