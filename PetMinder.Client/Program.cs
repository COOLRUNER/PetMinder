using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PetMinder.Client;
using PetMinder.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
var backendUrl = builder.Configuration["BackendUrl"];
if (string.IsNullOrWhiteSpace(backendUrl))
{
    backendUrl = builder.HostEnvironment.BaseAddress;
}

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri(backendUrl);
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendUrl) });

builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<SitterProfileService>();
builder.Services.AddScoped<FileUploadService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = clientFactory.CreateClient("AuthApi");
    return new FileUploadService(httpClient);
});
builder.Services.AddScoped<AuthService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = clientFactory.CreateClient("AuthApi");
    var localStorage = sp.GetRequiredService<Blazored.LocalStorage.ILocalStorageService>();
    var authStateProvider = sp.GetRequiredService<AuthenticationStateProvider>();
    return new AuthService(httpClient, localStorage, authStateProvider);
});
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<SitterAvailabilityService>();

builder.Services.AddScoped<SitterSearchService>();

builder.Services.AddScoped<BookingService>();

builder.Services.AddScoped<ChatService>();

builder.Services.AddScoped<ConversationService>();

builder.Services.AddScoped<AddressService>();

builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<RecommendationService>();

builder.Services.AddScoped<PetMinder.Client.Services.AdminService>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = clientFactory.CreateClient("AuthApi");
    return new PetMinder.Client.Services.AdminService(httpClient);
});
await builder.Build().RunAsync();
