using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Trade.Dashboard.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var baseUrl = builder.Configuration["DashboardApi:BaseUrl"]
    ?? throw new InvalidOperationException("Dashboard API base URL is missing.");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(baseUrl, UriKind.Absolute),
});
builder.Services.AddScoped<ITradeDashboardApiClient, TradeDashboardApiClient>();

await builder.Build().RunAsync();
