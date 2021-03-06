using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PM.MVC.Infrastructure;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;
using PM.MVC.Models.Repositories;
using PM.MVC.Models.Services;
using PM.MVC.Models;

namespace PM.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PMAppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AzureDefaultConnection")));
            services.AddIdentity<IdentityResource, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddDefaultUI()
            .AddEntityFrameworkStores<PMAppDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddScoped<IRepository<Project>, ProjectRepository>();
            services.AddScoped<IRepository<Qualification>, QualificationRepository>();
            services.AddScoped<IRepository<Skill>, SkillRepository>();
            services.AddScoped<IQualificationService<Project>, ProjectQualificationService>();
            services.AddScoped<IQualificationService<IdentityResource>, IdentityResourceQualificationService>();
            services.AddScoped<IResourceService<Project>, ProjectResourceService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IExcelService<Qualification>, QualificationExcelService>();
            services.AddScoped<IExcelService<IdentityResource>, IdentityResourceExcelService>();
            services.AddScoped<SummaryService>();

            services.AddControllersWithViews()
                   .AddNewtonsoftJson(options =>
                   {
                       options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                   });

            services.AddRazorPages();

            services.AddSignalR()
                    .AddNewtonsoftJsonProtocol(p =>
                    {
                        p.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<SignalServer>("/signalServer");
            });

            //DbInitializer.Seed(app).Wait();
        }
    }
}