using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Timelogger.Projects;
using Timelogger.Repository;
using FluentValidation;
using Timelogger.Api.Validators;
using Timelogger.Foundations.DateTime;
using Timelogger.TimeRegistrations;

namespace Timelogger.Api
{
	public class Startup
	{
		private readonly IWebHostEnvironment _environment;
		public IConfigurationRoot Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			_environment = env;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("e-conomic interview"));
			services.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.AddDebug();
			});

			services.AddMvc(options => options.EnableEndpointRouting = false);

			if (_environment.IsDevelopment())
			{
				services.AddCors();
			}

            services.AddScoped<IValidator<TimeRegistration>, TimeRegistrationsValidator>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<TimeRegistration>());
            services.AddTransient<IProjectsRepository, ProjectsRepository>();
            services.AddTransient<ITimeRegistrationsRepository, TimeRegistrationsRepository>();
        }

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseCors(builder => builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.SetIsOriginAllowed(origin => true)
					.AllowCredentials());
			}

			app.UseMvc();


			var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
			using (var scope = serviceScopeFactory.CreateScope())
			{
				SeedDatabase(scope);
			}
		}

		private static void SeedDatabase(IServiceScope scope)
		{
			var context = scope.ServiceProvider.GetService<ApiContext>();

            var interview = new Project
            {
                Id = 1,
                Name = "e-conomic Interview",
                Deadline = new DateTime(2023, 10, 13),
                IsCompleted = true
            };

            context.Projects.Add(interview);

            context.Projects.Add(new Project
            {
                Id = 2,
                Name = "MenuGenerator",
                Deadline = new DateTime(2023, 09, 29),
                IsCompleted = false
            });

            var chatGpt = new Project
            {
                Id = 3,
                Name = "ChatGPT",
                Deadline = new DateTime(2023, 09, 01),
                IsCompleted = false
            };

            context.Projects.Add(chatGpt);

            context.TimeRegistrations.Add(new TimeRegistration()
            {
                Project = interview,
                CreatedAtUtc = DateTime.UtcNow.Date,
                Minutes = 180,
                ValueDate = new DateTime(2023, 10, 10)
            });

            context.TimeRegistrations.Add(new TimeRegistration()
            {
                Project = interview,
                CreatedAtUtc = DateTime.UtcNow.Date,
                Minutes = 150,
                ValueDate = new DateTime(2023, 10, 11)
            });

            context.TimeRegistrations.Add(new TimeRegistration()
            {
                Project = interview,
                CreatedAtUtc = DateTime.UtcNow.Date,
                Minutes = 210,
                ValueDate = new DateTime(2023, 10, 12)
            });
            context.TimeRegistrations.Add(new TimeRegistration()
            {
                Project = chatGpt,
                CreatedAtUtc = DateTime.UtcNow.Date,
                Minutes = 45,
                ValueDate = new DateTime(2023, 10, 12)
            });

            context.TimeRegistrations.Add(new TimeRegistration()
            {
                Project = chatGpt,
                CreatedAtUtc = DateTime.UtcNow.Date,
                Minutes = 90,
                ValueDate = new DateTime(2023, 10, 12)
            });

            context.SaveChanges();
		}
	}
}