using Trade.Dashboard.Client.Pages;
using Trade.Dashboard.Components;
using Trade.Dashboard.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddScoped<HttpClient>(_ =>
{
    var baseUrl = builder.Configuration["DashboardApi:BaseUrl"]
        ?? throw new InvalidOperationException("Dashboard API base URL is missing.");

    return new HttpClient
    {
        BaseAddress = new Uri(baseUrl, UriKind.Absolute),
    };
});
builder.Services.AddScoped<ITradeDashboardApiClient, TradeDashboardApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Trade.Dashboard.Client._Imports).Assembly);

app.Run();
