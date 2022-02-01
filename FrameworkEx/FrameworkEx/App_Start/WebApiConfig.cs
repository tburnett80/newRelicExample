using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Datadog.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Serilog.Extensions.Logging;

namespace FrameworkEx
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //read any app settings keys from the web.config that may exist.
            var appsettings = ConfigurationManager.AppSettings.AllKeys.ToDictionary(k => k, k => ConfigurationManager.AppSettings[k]);

            // Web API configuration and services
            var cfg = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddEnvironmentVariables() 
                .AddInMemoryCollection(appsettings)
                .Build();

            //setup serilog to send to DD
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.DatadogLogs(cfg[EnvironmentVariableConsts.DataDogApiKey], service: cfg[EnvironmentVariableConsts.ApplicationNameKey],
                    configuration: new DatadogConfiguration { Url = cfg[EnvironmentVariableConsts.DataDogLogEndpoint] })
                .CreateLogger();

            //Build the dep resolver.
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(cfg)
                .AddSingleton<ILogger>(log)
                .AddControllersAsServices(typeof(WebApiConfig).Assembly.GetExportedTypes()
                    .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                    .Where(t => typeof(IController).IsAssignableFrom(t) || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)))
                .BuildServiceProvider();

            config.DependencyResolver = new DefaultDependencyResolver(services);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }

    public class DefaultDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        protected IServiceProvider _serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyResolver(_serviceProvider.CreateScope().ServiceProvider);
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }
    }

    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services, IEnumerable<Type> controllerTypes)
        {
            foreach (var type in controllerTypes)
                services.AddTransient(type);
            
            return services;
        }
    }
}
