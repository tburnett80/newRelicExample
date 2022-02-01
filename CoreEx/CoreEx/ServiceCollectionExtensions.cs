using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Datadog.Logs;
using System;
using System.IO;
using System.Reflection;

namespace CoreEx
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection WireupSingletons(this IServiceCollection services, IConfiguration config, IHostEnvironment host)
        {
            services.AddSingleton<IConfiguration>(config)
                    .AddSingleton<IHostEnvironment>(host);
                    //.AddScoped<IRequestContext, XtraRequestContext>();

            return services;
        }

        /// <summary>
        /// Will setup the Xtra logging.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static IServiceCollection ConfigureLogging(this IServiceCollection services)
        {
            //Get temp config handle.
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            //services.AddLogging(cfg => {
            //    cfg.AddSoaLogFile();
            //    cfg.SetMinimumLevel(LogLevel.Trace);
            //    cfg.AddDebug();
            //    cfg.AddTraceSource("");
            //}).Configure<ThreadSafeSoaLogFileOptions>(opt => config.ConfigureLogOptions(opt));

            //Setup serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.DatadogLogs(config[EnvironmentVariableConsts.DataDogApiKey], service: config[EnvironmentVariableConsts.ApplicationNameKey],
                    configuration: new DatadogConfiguration { Url = config[EnvironmentVariableConsts.DataDogLogEndpoint] })
                .CreateLogger();

            return services;
        }

        /// <summary>
        /// Wires up the automapper configurations by loading profile in assemblies. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            //Add the mapping assembly's type here. 
            //services.AddAutoMapper(
            //    Assembly.GetAssembly(typeof(PortalDtoModelAutoMapperProfile)),
            //    Assembly.GetAssembly(typeof(PortalModelAutoMapperProfile))
            //);

            return services;
        }

        /// <summary>
        /// Sets up Swagger for non prod environments.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        internal static IServiceCollection BootstrapSwagger(this IServiceCollection services)
        {
            var env = services.BuildServiceProvider().GetRequiredService<IHostEnvironment>();

            if (env.IsProduction())
                return services;

            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            //Swagger is going to require an auth token, so lets wireup the http client factory that handles this.
            //services.AddHttpClient(EnvironmentVariableConsts.SwaggerTokenApiBaseKey, cli =>
            //{
            //    cli.BaseAddress = new Uri(config[EnvironmentVariableConsts.SwaggerTokenApiBaseKey]);
            //    cli.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            //    {
            //        NoCache = true,
            //        NoStore = true,
            //        MaxAge = new TimeSpan(0),
            //        MustRevalidate = true
            //    };
            //}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            //{
            //    AllowAutoRedirect = true,
            //    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            //    Credentials = new NetworkCredential(config[EnvironmentVariableConsts.SwaggerTestUserKey], config[EnvironmentVariableConsts.SwaggerTestPassKey], config[EnvironmentVariableConsts.SwaggerTestDomainKey])
            //});

            //setup swagger
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = config[EnvironmentVariableConsts.SwaggerTitleKey],
                    Description = config[EnvironmentVariableConsts.SwaggerDescriptionKey],
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = config[EnvironmentVariableConsts.SwaggerEmailNameKey],
                        Email = config[EnvironmentVariableConsts.SwaggerEmailAddressKey]
                    }
                });

                cfg.DescribeAllParametersInCamelCase();
                cfg.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

                //var scheme = new OpenApiSecurityScheme
                //{
                //    Name = "JWT Auth",
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "bearer",
                //    BearerFormat = "JWT",
                //    Reference = new OpenApiReference
                //    {
                //        Id = JwtBearerDefaults.AuthenticationScheme,
                //        Type = ReferenceType.SecurityScheme
                //    }
                //};

                //cfg.AddSecurityDefinition(scheme.Reference.Id, scheme);
                //cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    { scheme, new List<string>() },
                //});
            });

            return services;
        }

        //IMvcBuilder Versions
        internal static IMvcBuilder BootstrapSwagger(this IMvcBuilder builder)
        {
            builder.Services.BootstrapSwagger();
            return builder;
        }

        internal static IMvcBuilder AddCors(this IMvcBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            if (config[EnvironmentVariableConsts.CorsOriginsKey] != null)
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.WithOrigins(config[EnvironmentVariableConsts.CorsOriginsKey].Split(','))
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        });
                });

            return builder;
        }

        internal static IMvcBuilder AddResponseCaching(this IMvcBuilder builder)
        {
            builder.Services.AddResponseCaching();
            return builder;
        }

        internal static IMvcBuilder AddResponseCompression(this IMvcBuilder builder, Action<ResponseCompressionOptions> configureOptions)
        {
            builder.Services.AddResponseCaching();
            return builder;
        }

        internal static IMvcBuilder Configure<TOptions>(this IMvcBuilder builder, Action<TOptions> configureOptions) where TOptions : class
        {
            builder.Services.Configure<TOptions>(configureOptions);
            return builder;
        }

        internal static IMvcBuilder Configure<TOptions>(this IMvcBuilder builder, string name, Action<TOptions> configureOptions) where TOptions : class
        {
            builder.Services.Configure<TOptions>(name, configureOptions);
            return builder;
        }
    }
}
