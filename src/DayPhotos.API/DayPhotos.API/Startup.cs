using AutoMapper;
using DayPhoto.API.Entities;
using DayPhoto.API.Repositories;
using DayPhotos.API.BackgroundServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace DayPhoto.API
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
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
            services.AddScoped<AppUnitOfWork>();
            services.AddTransient(typeof(IDCRepositoryBase<,>), typeof(AppRepositoryBase<,>));

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddDistributedMemoryCache();

            services.AddTransient<PhotoCollectionJob>();
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-DayPhotos";
                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    options.AllowDefaultConstructor = true;
                    options.CreateScope = true;
                });
                q.ScheduleJob<PhotoCollectionJob>(trigger => trigger
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithInterval(System.TimeSpan.FromHours(2)).RepeatForever())
                //.WithCronSchedule("0 0 3 * * ?") //3AM of every day
                );
            });
            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API Docs",
                    Version = "v1",
                });
            });
            #endregion
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor| ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseSwagger();
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
