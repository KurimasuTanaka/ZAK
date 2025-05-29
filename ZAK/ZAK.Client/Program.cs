using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
namespace ZAK.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddBlazorBootstrap();
        await builder.Build().RunAsync();
    }
}
