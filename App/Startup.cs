using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DbUp;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using CloudSeedApp;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CloudSeedApp
{
    public class Startup
    {
        public Startup(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            WebHostEnvironment = webHostEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationProvider = new ConfigurationProvider(
                this.Configuration,
                this.WebHostEnvironment
            );
            var dbConnectionString = DatabaseConnectionStringProvider.GetConnectionStringFromConfigurationProvider(
                configurationProvider
            );

            var databaseUpgradeResult = UpgradeDatabase(dbConnectionString);
            if(databaseUpgradeResult != 0) {
                throw new Exception("Failed upgrading database!");
            }

            services.AddMediatR(typeof(Startup));
            services.AddControllers();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            // Singleton
            services.AddSingleton<ConfigurationProvider>(s =>
            {
                return configurationProvider;
            });
            services.AddSingleton<TSimpleMemoryCache<Checkout>>(s => {
                return new TSimpleMemoryCache<Checkout>(
                    new SimpleMemoryCacheOptions(
                        1000,
                        TimeSpan.FromMinutes(10),
                        TimeSpan.FromMinutes(60)
                    )
                );
            });
            services.AddSingleton<TSimpleMemoryCache<User>>(s => {
                return new TSimpleMemoryCache<User>(
                    new SimpleMemoryCacheOptions(
                        1000,
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromMinutes(1)
                    )
                );
            });
            services.AddSingleton<IEmailProvider, PostmarkEmailProvider>(s => {
                return new PostmarkEmailProvider(
                    s.GetRequiredService<ConfigurationProvider>()
                );
            });
            services.AddSingleton<ProductDataProvider>(s => {
                return new ProductDataProvider(
                    this.WebHostEnvironment.IsDevelopment() 
                        ? ProductConfiguration.GetTestProducts()
                        : ProductConfiguration.GetProductionProducts()
                );
            });
            services.AddSingleton<INowProvider, UtcNowProvider>();
            services.AddSingleton<JwtAuthenticationProcessor>();

            // Scoped
            services.AddDbContext<CloudSeedAppDatabaseContext>(
                options => options.UseNpgsql(
                    dbConnectionString
                )
            );
            services.AddScoped<CheckoutDataProvider>();
            services.AddScoped<CheckoutProcessor>();
            services.AddScoped<ICurrentUserService, CurrentUserHttpContextProcessor>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IOrderService, StripeOrderService>();
            services.AddScoped<StripeCheckoutProcessor>();
            services.AddScoped<SubscriptionDataProvider>();
            services.AddScoped<UserDataProvider>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "app", Version = "v1" });
            });

            // Authentication
            services.AddIdentity<User, ApplicationRole>()
                .AddUserStore<UserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.TokenValidationParameters = JwtAuthenticationProcessor.GetJwtAuthenticationTokenValidationParameters(
                        configurationProvider.JWT_SIGNING_KEY
                    );
                });
            services.AddAuthorization(options => {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configurationProvider = new ConfigurationProvider(
                this.Configuration,
                this.WebHostEnvironment
            );

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-development");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));

                app.UseCors(
                    builder => {
                        builder.WithOrigins(
                            // Web default
                            "https://localhost:5000",
                            "http://localhost:5000",
                            "https://host.docker.internal:5000",
                            "http://host.docker.internal:5000",
                            // App default
                            "https://localhost:5001",
                            "http://localhost:5001",
                            "https://host.docker.internal:5001",
                            "http://host.docker.internal:5001"
                        ).AllowAnyMethod()
                        .AllowAnyHeader();
                    }
                );
            } else {
                app.UseExceptionHandler("/error");

                app.UseCors(
                    builder => {
                        builder.WithOrigins(
                            // CHANGEME - add your production domains here
                            configurationProvider.WEB_BASE_URL
                        ).SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    }
                );
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static int UpgradeDatabase(string connectionString) {            
            try {
                EnsureDatabase.For.PostgresqlDatabase(connectionString);
            } catch(Exception) {
                Console.WriteLine("Could not connect to database - delaying before retry");
                Thread.Sleep(5000);
                EnsureDatabase.For.PostgresqlDatabase(connectionString);
            }
            
            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                // In the form of LineTimesServer.Source.DataModels.DatabaseUpgradeScripts.DatabaseUpgradeScript000001-PopulateSentinelTable.sql
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    (string scriptName) => {
                        // Console.WriteLine($"DBUp scriptName: {scriptName}");
                        return scriptName.Contains("DatabaseUpgradeScripts.DatabaseUpgradeScript");
                    })
                .WithTransactionPerScript()
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                throw result.Error;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value: "Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
