using AirandWebAPI.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.SqlServer;
using AutoMapper;
using AirandWebAPI.Helpers;
using AirandWebAPI.Services.Concrete;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Services;
using AirandWebAPI.Models.Auth;
using TheHangout.Services;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;
using Microsoft.Extensions.FileProviders;
using System.IO;
using AirandWebAPI.Models.Company;

namespace AirandWebAPI
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
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson();
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            //automapper
            services.AddAutoMapper(typeof(Startup));

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ICompanyService, CompanyService>();


            services.AddScoped<INotification, NotificationService>();
            services.AddScoped<IMailer, MailerService>();
            services.AddScoped<ISmsService, SmsService>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //validation services
            services.AddScoped<IValidation<RegisterModel>, UserRegistrationValidation>();
            services.AddScoped<IValidation<RideOrderRequest>, DispatchRequestValidation>();
            services.AddScoped<IValidation<Coordinates>, CoordinatesDataValidation>();
            services.AddScoped<IValidation<ChangeStatusVM>, ChangeStatusValidation>();
            services.AddScoped<IValidation<CreateCompanyVM>, CreateCompanyValidation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = "/Assets"
            });
        }
    }
}
