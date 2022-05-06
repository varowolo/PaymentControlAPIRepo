using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using PaymentControlAPI.Model;
using PaymentControlAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaymentControlAPI.Utilities;
using EmailService;
using PaymentControlAPI.BLL;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PaymentControlAPI
{
    public class Startup
    {        

        public Startup(Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)//;

                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", reloadOnChange: true, optional: true)
               .AddEnvironmentVariables();
            
            Configuration = builder.Build(); // load all file config to Configuration property 
            AppSettings.Instance.SetConfiguration(Configuration);

        }
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddMvc();

            //// Add functionality to inject IOptions<T>
            //services.AddOptions();

            //// Add our Config object so it can be injected
            services.AddHostedService<SendLoggedMail>();
            services.AddScoped<SendEmail>();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        services
     //.AddEntityFrameworkSqlServer()
     //.AddDbContext<PaymentDBContext>(opt =>
     .AddDbContext<PaymentControlDBContext>(opt =>
       opt.UseSqlServer(
           Configuration.GetConnectionString("MyConnection")           
           )
           );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PaymentControl API",
                    Description = "Payment Integration",
                    TermsOfService = new Uri("https://hbng.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Heritage Bank Limited",
                        Email = "tosinf@yahoo.com",
                        Url = new Uri("http://hbng.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under Heritage Bank Limited License",
                        Url = new Uri("http://hbng.com/terms"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();

            //app.UseHttpsRedirection();
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentControl API"); //Local machine
                c.SwaggerEndpoint("/pcadminui/swagger/v1/swagger.json", "PaymentControl API"); //Server
                //c.SwaggerEndpoint("/localhost/pcadminui/swagger/v1/swagger.json", "PaymentControl API"); //localhost

            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
