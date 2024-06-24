using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTest;

public class CustomWebApplicationFactory:WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(temp =>
                temp.ServiceType == typeof(DbContextOptionsBuilder<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            builder.UseEnvironment("test");
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseInMemoryDatabase("dataBaseForTesting");
            });
        }
            );
    }
}