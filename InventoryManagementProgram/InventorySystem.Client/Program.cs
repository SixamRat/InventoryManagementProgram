using InventorySystem.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Bygg API-adressen dynamiskt från webbläsarens hostname
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

// HttpClient MED auth 
builder.Services.AddHttpClient("InventoryAPI",
    client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler(sp =>
    {
        var provider = sp.GetRequiredService<IAccessTokenProvider>();
        var navigation = sp.GetRequiredService<NavigationManager>();
        var handler = new AuthorizationMessageHandler(provider, navigation);
        handler.ConfigureHandler(authorizedUrls: new[] { apiBaseUrl });
        return handler;
    });

// Standard HttpClient med auth
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("InventoryAPI"));

await builder.Build().RunAsync();