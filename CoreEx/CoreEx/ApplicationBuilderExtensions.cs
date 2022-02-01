using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CoreEx
{
    internal static class ApplicationBuilderExtensions
    {
        internal static void EnableSwagger(this IApplicationBuilder app, IHostEnvironment env, IConfiguration config)
        {
            if (env.IsProduction())
                return;

            // Serves generated swagger document as JSON endpoint. 
            app.UseSwagger();

            // Serves the Swagger UI
            app.UseSwaggerUI(c =>
            {
                // specifying the Swagger JSON endpoint.
                c.SwaggerEndpoint("/swagger/v1/swagger.json", config[EnvironmentVariableConsts.ApplicationNameKey]);

                //Set this to come from the projects root
                c.RoutePrefix = string.Empty;
            });
        }

        internal static IApplicationBuilder UseExceptionHandlerLogger(this IApplicationBuilder app)
        {
            //app.UseExceptionHandler(a => a.Run(async context =>
            //{
            //    var log = context.RequestServices.GetRequiredService<ILogger>();
            //    var rctx = context.RequestServices.GetRequiredService<IRequestContext>().Data;

            //    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            //    var exception = exceptionHandlerPathFeature.Error;

            //    log.LogError(exception, $"'{rctx.CorrelationId}' - Exception handlering triggered: {exception.Message}");
            //    await context.Response.WriteAsJsonAsync(new { correlation_id = rctx.CorrelationId, message = "There was an error with this request. Please use the corelation id to reference this error." });
            //}));

            return app;
        }
    }
}
