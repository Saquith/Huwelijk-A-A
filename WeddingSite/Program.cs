using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeddingSite;
using WeddingSite.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// HttpClient base address is the client base; requests to "api/..." will be same-origin when hosted from the backend.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<PhotoService>();
builder.Services.AddScoped<EmailService>();

await builder.Build().RunAsync();