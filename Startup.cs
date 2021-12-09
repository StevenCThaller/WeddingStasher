using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WeddingPhotos.Models.Db;
using WeddingPhotos.Models;
using WeddingPhotos.Services;
using System.Net;
using WeddingPhotos.Attributes;

namespace WeddingPhotos
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            // services.AddHsts(options => 
            // {
            //     options.Preload = true;
            //     options.IncludeSubDomains = true;
            //     options.MaxAge = TimeSpan.FromDays(30);
            // });
            // services.AddHttpsRedirection(options => 
            // {
            //     options.RedirectStatusCode = (int) HttpStatusCode.TemporaryRedirect;
            //     options.HttpsPort = 443;
            // });
            services.AddDbContext<WPContext>(options => options.UseMySql(Configuration["DBInfo:ConnectionString"]));
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<IGuestBookService, GuestBookService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // System.Console.WriteLine(AppSettings.appSettings.Emails[0]);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
        }
    }
}
