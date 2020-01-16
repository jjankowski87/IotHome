using System;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IotHomeService.App.Configuration;
using IotHomeService.App.Services;
using IotHomeService.App.Services.Interfaces;
using IotHomeService.Model;
using IotHomeService.Services;
using IotHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IotHomeService.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton(GetConfiguration<StorageConfiguration>("StorageConfiguration"));
            services.AddSingleton(GetConfiguration<RawAppConfiguration>("AppConfiguration"));
            services.AddSingleton<AppConfiguration>();

            services.AddSingleton<IReadingsNotifier, ReadingsNotifier>();
            services.AddScoped<IStorageExplorer, StorageExplorer>();
            services.AddScoped<IReadingsService, ReadingsService>();
            services.AddScoped<IStorageHelper, StorageHelper>();

            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }

        private TConfiguration GetConfiguration<TConfiguration>(string sectionName)
        {
            var config = (TConfiguration)Activator.CreateInstance(typeof(TConfiguration));
            Configuration.GetSection(sectionName).Bind(config);

            return config;
        }
    }
}
