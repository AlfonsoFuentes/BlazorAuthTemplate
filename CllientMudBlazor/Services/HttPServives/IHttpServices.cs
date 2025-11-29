using Shared.Dtos.General;
using Shared.Dtos.Suppliers;
using System.Net.Http.Json;

namespace CllientMudBlazor.Services.HttPServives
{
    public interface IHttpServices
    {
        Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class;
        Task<bool> PostForValidationAsync<TRequest>(TRequest request) where TRequest : class;
    }
    public partial class HttpServices : IHttpServices
    {
        private readonly HttpClient _httpClient;
        private readonly ISnackBarService _snackbarService; // ✅ Tu servicio

        public HttpServices(IHttpClientFactory httpClientFactory, ISnackBarService snackbarService)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _snackbarService = snackbarService;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request)
     where TRequest : class
     where TResponse : class
        {
            try
            {
                var endpoint = request.GetType().Name;
                var response = await _httpClient.PostAsJsonAsync(endpoint, request);

                // ✅ Caso 1: Éxito (200 OK)
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TResponse>()
                        ?? Activator.CreateInstance<TResponse>(); // fallback seguro

                    if (result is GeneralDto generalDto)
                        _snackbarService.ShowMessage(generalDto);

                    return result;
                }

                // ✅ Caso 2: Error del servidor (4xx, 5xx)
                var errorContent = await response.Content.ReadAsStringAsync();
                var message = $"Error {response.StatusCode}: {errorContent}".Truncate(200); // evita mensajes gigantes

                _snackbarService.ShowError(message);
                return Activator.CreateInstance<TResponse>();
            }
            catch (HttpRequestException ex)
            {
                // ✅ Caso 3: Error de red (timeout, DNS, sin conexión)
                var message = ex.InnerException?.Message ?? ex.Message;
                _snackbarService.ShowError($"Connection error: {message}");
                return Activator.CreateInstance<TResponse>();
            }
            catch (TaskCanceledException)
            {
                // ✅ Caso 4: Timeout
                _snackbarService.ShowError("Request timed out. Please try again.");
                return Activator.CreateInstance<TResponse>();
            }
            catch (Exception ex)
            {
                // ✅ Caso 5: Otros errores (JSON inválido, etc.)
                _snackbarService.ShowError($"Unexpected error: {ex.Message}");
                return Activator.CreateInstance<TResponse>();
            }
        }
        // ✅ Nuevo método: solo para validaciones (devuelve bool, sin Snackbar)
        public async Task<bool> PostForValidationAsync<TRequest>(TRequest request)
            where TRequest : class
        {
            var endpoint = request.GetType().Name;
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);

            // ✅ Solo 200 OK es éxito; cualquier otro código → false
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<GeneralDto<bool>>();
                return result?.Suceeded == true && result.Data == true;
            }

            return false;
        }
    }
}
