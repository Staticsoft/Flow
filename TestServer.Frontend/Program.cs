using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Staticsoft.TestServer.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(p => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddRadzenComponents();

await builder.Build().RunAsync();