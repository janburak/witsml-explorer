using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using WitsmlExplorer.Api;
using WitsmlExplorer.Api.Configuration;
using WitsmlExplorer.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddJsonFile("mysettings.json", true, true)
        .AddJsonFile("config.json", true, true)
        .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Startup>();
}
else
{
    string keyVault = builder.Configuration[ConfigConstants.KVWitsmlServerCreds];
    bool useOAuth2 = builder.Configuration[ConfigConstants.OAuth2Enabled] == "True";
    builder.Configuration.AddAzureWitsmlServerCreds(keyVault, useOAuth2);
}

builder.Host.ConfigureLogging(logging => logging.ClearProviders());
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

Startup startup = new(builder.Configuration);
startup.ConfigureServices(builder.Services);

WebApplication app = builder.Build();
app.ConfigureApi(builder.Configuration);

startup.Configure(app, app.Environment);


app.Run();
