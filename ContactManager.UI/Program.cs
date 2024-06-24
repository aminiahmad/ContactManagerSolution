using CRUDExample.Middleware;
using CRUDExample.StartupExtensions;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

//serilog 
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service, LoggerConfiguration configure) =>
    {
        configure.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(service);
    }
);
//logging
/*builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventLog();
});*/

builder.Services.ConfigureServices(builder.Configuration);

// custom http req and res in log 
builder.Services.AddHttpLogging(option =>
{
    option.LoggingFields =
        HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;
});
var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error"); //redirect to /error in home controller

    app.UseExceptionHandleMiddleware();

}
app.UseSerilogRequestLogging();
//app.Logger.LogDebug("Debug-message");
//app.Logger.LogInformation("Information-message");
//app.Logger.LogWarning("Warning-message");
//app.Logger.LogError("Error-message");
//app.Logger.LogCritical("Critical-message");

if (builder.Environment.IsEnvironment("test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot","Rotativa");
}

//enable https on client
app.UseHsts();
app.UseHttpsRedirection();
//----------------------
app.UseStaticFiles();
app.UseRouting();//Identifying Action Methods Based Route
app.UseAuthentication();//Reading Identity cookies
app.UseAuthorization(); // Validation Access permissions of the user
app.MapControllers();//Execute The Filter Pipeline (Action + Filters)
app.Run();
app.UseHttpLogging();

//conventional Routing 
/*app.UseEndpoints(endPoint =>
{
    endPoint.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}"
    );
});*/

public partial class Program
{ }// make the auto generated program accessible programmatically