using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FullStackTask.Presentation.VueCoreConnection;
using FullStackTaskService;
using AutoMapper;
using System.Reflection;

namespace FullStackTask.Presentation
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
            services.AddControllers();
            // connect vue app - middleware
            services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");


            services.AddMvc().AddNewtonsoftJson();

            services.AddMemoryCache();

            services.AddScoped<IWeatherApiManagement, WeatherApiManagement>
                (item => new WeatherApiManagement(new Uri(Configuration.GetSection("ApiSettings").GetValue<string>("ApiBaseUrl")),
                Configuration.GetSection("ApiSettings").GetValue<string>("ApiKey")));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // use middleware and launch server for Vue
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment())
                {
                    
                    spa.UseVueDevelopmentServer();
                }
            });
        }
    }
}
