using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeddingSite;
using WeddingSite.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PhotoService>();

// Register EmailService and pass configured endpoint from appsettings.json.
// Replace the endpoint value in wwwroot/appsettings.json with your form/email API endpoint.
var emailEndpoint = builder.Configuration["EmailApi:Endpoint"] ?? string.Empty;
builder.Services.AddScoped(sp => new EmailService(sp.GetRequiredService<HttpClient>(), emailEndpoint));

await builder.Build().RunAsync();