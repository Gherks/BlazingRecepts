using BlazingRecept.Server.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.AddBlazingReceptDbContext(builder.Configuration);
builder.AddBlazingReceptServices();
builder.AddBlazingReceptRepositories();
builder.Services.AddSwaggerGen();

// Configure Serilog
builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) => loggerConfiguration
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(hostBuilderContext.Configuration));

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

app.MapRazorPages();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
