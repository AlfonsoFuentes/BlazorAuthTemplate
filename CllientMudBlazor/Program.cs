using BlazorDownloadFile;
using Blazored.LocalStorage;
using CllientMudBlazor;
using CllientMudBlazor.Services;
using CllientMudBlazor.Services.HttPServives;
using CllientMudBlazor.Services.NotificationServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string ClientName = "API";
builder.Services.AddTransient<AuthenticationHeaderHandler>();
builder.Services.AddHttpClient(ClientName, (sp, client) =>
{
    client.DefaultRequestHeaders.AcceptLanguage.Clear();
    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.EnableIntercept(sp);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddHttpClientInterceptor();
builder.Services.AddMudServices();
builder.Services.AddScoped<ISnackBarService, SnackBarService>();
builder.Services.AddScoped<IHttpServices, HttpServices>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddSingleton<ProjectNotificationService>();
builder.Services.AddBlazorDownloadFile();
builder.Services.AddScoped<HttpInterceptorService>();
await builder.Build().RunAsync();
