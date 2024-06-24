using ContactManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilter;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContract;
using Service;
using ServiceContract;

namespace CRUDExample.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection service, IConfiguration configure)
    {
        service.AddTransient<ResponseHeaderActionFilter>();

        //add global filter
        service.AddControllersWithViews(option =>
        {
            //option.Filters.Add<ResponseHeaderActionFilter>(5);
            var logger= service.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
            option.Filters.Add(new ResponseHeaderActionFilter(logger){ Key = "my-key-fromGlobal", Value = "my-value-fromGlobal",Order = 2});
            //برای هر درخواست از نوع post که ممکنه مخرب باشه اگر auto نبود برای همه درخواست ها اثر میذاشت که ما نمیخوایم اینو
            option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        //add service into IoC Container
        service.AddScoped<ICountryGetterService, CountryGetterService>();
        service.AddScoped<ICountryAdderService, CountryAdderService>();
        service.AddScoped<ICountryUploaderService, CountryUploaderService>();

        //open close principle - Solid
        service.AddScoped<IPersonGetterService, PersonsGetterFewExcelFieldService>(); // interface // Better // don't solid principle Liskov s p

        //service.AddScoped<IPersonGetterService, PersonsGetterFewExcelFieldService>(); // inheritance

        // inject PersonsGetterService in personsGetterFewExcelFieldService class so add service in configure
        service.AddScoped<PersonsGetterService, PersonsGetterService>();
        

        service.AddScoped<IPersonAdderService, PersonsAdderService>();
        service.AddScoped<IPersonSorterService, PersonsSorterService>();
        service.AddScoped<IPersonDeleterService, PersonsDeleterService>();
        service.AddScoped<IPersonUpdaterService, PersonsUpdaterService>();
        service.AddScoped<IPersonsRepository, PersonsRepository>();
        service.AddScoped<ICountriesRepository, CountriesRepository>();

        //ServiceFilter
        // by default typeFilter is transient
        service.AddTransient<PersonsListActionFilter>();

        // add service DbContext with use sql server db  ---  entity by default use scope service,
        // and we should be use scope service for using services of personsDbContext
        service.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configure["ConnectionString:DefaultConnection"]);
        });
        
        //---------------------
        // Enable Identity in this project
        //create table and store data to db
        service.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                option.Password.RequiredLength = 8;
                option.Password.RequireLowercase=true;
                option.Password.RequireUppercase=true;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireDigit=true;
                option.Password.RequiredUniqueChars = 3; // number of distinct character required in password
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            //provide new token be operation change password or register new user
            .AddDefaultTokenProviders()
            //add repository
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
        //----------------------

        //----------------------
        // user must be login 
        service.AddAuthorization(option =>
        {
            //enforce authorization policy (user must be authenticated) for all the action method 
            option.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); 

            //اگر کاربر لاگین کرد دوباره به صفحه لاگین و ثبت نام نتونه دسترسی داشته باشه
            option.AddPolicy("NotAuthorized", policy =>
            {
                policy.RequireAssertion(context => !(context.User.Identity!.IsAuthenticated));
            });
        });
        service.ConfigureApplicationCookie(option =>
        {
            option.LoginPath = "/Account/Login";
        });
        //-----------------------

        return service;
    }
}