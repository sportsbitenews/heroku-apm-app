using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SofttrendsAddon.Helpers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.IO;
using Serilog.Events;
using Microsoft.AspNetCore.Http.Features;

namespace SofttrendsAddon
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
                // Environment.SetEnvironmentVariable("DATABASE_URL", "postgres://kujqiiyoositgs:d939631a5c6338318a79483ed0b3d91a8432b7b439060f1c4a8f2248bdca4c8c@ec2-54-225-182-108.compute-1.amazonaws.com:5432/d13a7rjd3ncef");
            }
            //  Environment.SetEnvironmentVariable("DATABASE_URL", "postgres://kujqiiyoositgs:d939631a5c6338318a79483ed0b3d91a8432b7b439060f1c4a8f2248bdca4c8c@ec2-54-225-182-108.compute-1.amazonaws.com:5432/d13a7rjd3ncef");
            Configuration = builder.Build();
            // ConnectionString = Utilities.GetPGConnectionString(Utilities.GetEnvVarVal("DATABASE_URL"));
            //  Console.WriteLine("Connection String : " + ConnectionString);
            //set log setting
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "Logs\\log-{Date}.txt"), LogEventLevel.Debug,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}").
                CreateLogger();

        }

        public IConfigurationRoot Configuration { get; }
        public string ConnectionString { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //db setting
            // services.AddEntityFrameworkNpgsql().AddDbContext<SFTAddonContext>(options => options.UseNpgsql(ConnectionString, b => b.MigrationsAssembly("st-portfoliomanager")));

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10; //default 1024
                options.ValueLengthLimit = int.MaxValue; //not recommended value
                options.MultipartBodyLengthLimit = long.MaxValue; //not recommended value
            });
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromHours(5);
            });


            services.AddScoped<IRequestFactory, RequestFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            Utilities.AppServiceProvider = serviceProvider;
            Utilities.AppLoggerFactory = loggerFactory;
            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //Only one local or production
            //try
            //{
            //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //    {
            //        serviceScope.ServiceProvider.GetService<SFTAddonContext>().Database.Migrate();
            //    }
            //}
            //catch (Exception e)
            //{
            //    var msg = e.Message;
            //    var stacktrace = e.StackTrace;
            //}
            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
