using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ElsaGuides.ContentApproval.Client;
using ElsaGuides.ContentApproval.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client configurato per API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7001")
});

// MudBlazor
builder.Services.AddMudServices();

// Services
builder.Services.AddScoped<IDocumentService, DocumentService>();

await builder.Build().RunAsync();