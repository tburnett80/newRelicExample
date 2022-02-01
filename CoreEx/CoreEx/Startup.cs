using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreEx
{
    /// <inheritdoc />
    public class Startup
    {
        /// <inheritdoc />
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        /// <inheritdoc />
        public IConfiguration Configuration { get; }
        /// <inheritdoc />
        public IWebHostEnvironment Env { get; }

        /// <inheritdoc />
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.WireupSingletons(Configuration, Env)
                .ConfigureLogging()
                .ConfigureAutoMapper()
                .AddControllers()
                .AddNewtonsoftJson()
                .AddJsonOptions(opt =>
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true)
                .AddCors()
                .BootstrapSwagger()
                //.AddResponseCaching()
                //.AddResponseCompression(options =>       
                //    options.Providers.Add<GzipCompressionProvider>())
                //.Configure<GzipCompressionProviderOptions>(options =>
                //    options.Level = System.IO.Compression.CompressionLevel.Fastest)
                ;
        }

        /// <inheritdoc />
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            //app.UseResponseCaching();
            //app.UseResponseCompression();

            app.UseRouting();
            app.UseCors();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());

            app.EnableSwagger(env, Configuration);
        }
    }
}
