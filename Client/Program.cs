using BlazingRecept.Client;
using BlazingRecept.Client.Extensions;
using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Serilog;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.AddHttpClients();
    builder.AddServices();

    builder.Services.AddHxServices();
    builder.Services.AddHxMessenger();

    // Add Radzen services
    builder.Services.AddRadzenComponents();

    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .Enrich.WithProperty("ClientId", Guid.NewGuid().ToString("n"))
        .WriteTo.DurableHttpUsingFileSizeRolledBuffers(requestUri: builder.HostEnvironment.BaseAddress + "api/logs")
        .CreateLogger();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application startup error: {ex.Message}");
    Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}
