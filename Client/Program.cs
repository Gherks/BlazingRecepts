using BlazingRecept.Client;
using BlazingRecept.Client.Extensions;
using Blazored.Toast;
using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddHttpClients();
builder.AddServices();

builder.Services.AddHxServices();
builder.Services.AddHxMessenger();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://9fe5670d-f19b-454b-bfd1-1ba2180409d0/API.Access");
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("ClientId", Guid.NewGuid().ToString("n"))
    .WriteTo.DurableHttpUsingFileSizeRolledBuffers(requestUri: builder.HostEnvironment.BaseAddress + "api/logs")
    .CreateLogger();

await builder.Build().RunAsync();
