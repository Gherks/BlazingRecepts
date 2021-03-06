using Azure.Identity;
using BlazingRecept.Server.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.AddBlazingReceptDbContext(builder.Configuration);
builder.AddBlazingReceptServices();
builder.AddBlazingReceptRepositories();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

// Configure Serilog
builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) => loggerConfiguration
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(hostBuilderContext.Configuration));

// Configure Azure key vault
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["AzureKeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseMiddleware<CustomExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazingRecept API V1");
    });
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//if (app.Environment.IsDevelopment())
//    app.MapControllers().WithMetadata(new AllowAnonymousAttribute());
//else
//    app.MapControllers();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
