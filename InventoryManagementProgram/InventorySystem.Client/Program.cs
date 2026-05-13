using InventorySystem.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Bygg API-adressen från webbläsarens hostname
var currentHost = new Uri(builder.HostEnvironment.BaseAddress).Host;
var apiBaseUrl = $"https://{currentHost}:7232";

// MSAL-autentisering mot Entra ID
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(
        builder.Configuration["ApiSettings:Scope"]!
    );
});

// Konfigurera auth-handler att skicka token till API:et
builder.Services.AddScoped<AuthorizationMessageHandler>(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.ConfigureHandler(
        authorizedUrls: new[] { apiBaseUrl }
    );
    return handler;
});

// HttpClient MED auth
builder.Services.AddHttpClient("InventoryAPI",
    client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Standard HttpClient med auth
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("InventoryAPI"));

await builder.Build().RunAsync();