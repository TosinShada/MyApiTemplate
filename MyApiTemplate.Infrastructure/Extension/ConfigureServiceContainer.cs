using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MyApiTemplate.Domain.Settings;
using MyApiTemplate.Infrastructure.Configs;
using FluentValidation.AspNetCore;
using MyApiTemplate.Service.Contract;
using MyApiTemplate.Service.Implementation;
using System;
using System.Collections.Generic;
using FluentValidation;
using MyApiTemplate.Service.DTO.Request;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly;
using System.Net;
using System.Net.Http.Headers;

namespace MyApiTemplate.Infrastructure.Extension
{
    public static class ConfigureServiceContainer
    {
        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDateTimeService, DateTimeService>();
            serviceCollection.AddTransient<IAccountService, AccountService>();
            serviceCollection.AddTransient<IDBFactory, DBFactory>();
            serviceCollection.AddTransient<IClientFactory, ClientFactory>();
            serviceCollection.AddTransient<IRepository, Repository>();
        }

        public static void AddAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(MappingProfileConfiguration));
        }

        public static void AddValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IValidator<CreatePersonRequest>, CreatePersonRequestValidator>();
            serviceCollection.AddTransient<IValidator<UpdatePersonRequest>, UpdatePersonRequestValidator>();

            //Disable Automatic Model State Validation built-in to ASP.NET Core
            serviceCollection.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
        }

        public static void AddSwaggerOpenAPI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(setupAction =>
            {

                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = "Onion Architecture WebAPI",
                        Version = "1",
                        Description = "Through this API you can access customer details",
                        Contact = new OpenApiContact()
                        {
                            Email = "amit.naik8103@gmail.com",
                            Name = "Amit Naik",
                            Url = new Uri(" https://amitpnk.github.io/")
                        },
                        License = new OpenApiLicense()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = $"Input your Bearer token in this format - Bearer token to access this API",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }, new List<string>()
                    },
                });
            });

        }

        public static void AddController(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation();
        }

        public static void AddVersion(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        public static void AddHTTPPolicies(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var policyConfigs = new HttpClientPolicyConfiguration();
            configuration.Bind("HttpClientPolicies", policyConfigs);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(policyConfigs.RetryTimeoutInSeconds));

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(policyConfigs.RetryCount, _ => TimeSpan.FromMilliseconds(policyConfigs.RetryDelayInMs));

            var circuitBreakerPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .CircuitBreakerAsync(policyConfigs.MaxAttemptBeforeBreak, TimeSpan.FromSeconds(policyConfigs.BreakDurationInSeconds));

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            //Register a Typed Instance of HttpClientFactory for a Protected Resource
            //More info see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0

            serviceCollection.AddHttpClient<IClientFactory, ClientFactory>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiResourceBaseUrls:SampleApi"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
            .AddPolicyHandler(timeoutPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);
        }

        public static void AddHealthCheck(this IServiceCollection serviceCollection, AppSettings appSettings, IConfiguration configuration)
        {
            serviceCollection.AddHealthChecks()
                .AddUrlGroup(new Uri(appSettings.ApplicationDetail.ContactWebsite), name: "My personal website", failureStatus: HealthStatus.Degraded)
                .AddSqlServer(configuration.GetConnectionString("SQLDBConnectionString"));

            serviceCollection.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health Check", $"/healthz");
            }).AddInMemoryStorage();
        }

        public static void AddRequestRateLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // needed to load configuration from appsettings.json
            serviceCollection.AddOptions();
            // needed to store rate limit counters and ip rules
            serviceCollection.AddMemoryCache();

            //load general configuration from appsettings.json
            serviceCollection.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            serviceCollection.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            serviceCollection.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            serviceCollection.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
