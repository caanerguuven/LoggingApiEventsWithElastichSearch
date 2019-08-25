using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;


namespace LoggingApiExample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var elasticUri = Configuration["ElasticSearch:Uri"];
            var logFileAddress= Path.Combine(Directory.GetCurrentDirectory(), Configuration["ElasticSearch:FileName"]);
            Log.Logger = new LoggerConfiguration()
                         .Enrich.WithExceptionDetails()
                         .Enrich.WithMachineName()
                         .MinimumLevel.Override("Microsoft",LogEventLevel.Error)
                         .MinimumLevel.Override("System",LogEventLevel.Information)
                         .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                         {
                             CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                             AutoRegisterTemplate = true,
                             TemplateName = "serilog-events-template",
                             IndexFormat = "loggingapiexample-{0:yyyy.MM.dd}"
                         })
                         .WriteTo.RollingFile(Path.Combine(logFileAddress, "paycell-log-{0:yyyy.MM.dd}"))
                        .MinimumLevel.Verbose()
                        .CreateLogger();

        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddSerilog();

            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "MyApi/{controller=Home}/{action}"
                );
            });
        }
    }
}
