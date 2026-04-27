using InventorySystem.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// MSAL-autentisering mot Entra ID
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(
        builder.Configuration["ApiSettings:Scope"]!
    );
});

// HttpClient MED auth för senare
builder.Services.AddHttpClient("InventoryAPI",
    client => client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// HttpClient UTAN auth för test
builder.Services.AddHttpClient("PublicAPI",
    client => client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!));

// Standard just nu
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("PublicAPI"));

await builder.Build().RunAsync();