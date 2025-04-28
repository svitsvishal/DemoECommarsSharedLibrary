using eCommerceSharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceSharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext> (this IServiceCollection services , IConfiguration config , 
            string filename)  where TContext : DbContext
        {
            //Add generic DbContext
            services.AddDbContext<TContext>(option =>option.UseSqlServer(config.
                GetConnectionString("eCommerceConnectionString"),sqlserverOption => sqlserverOption.EnableRetryOnFailure()
            ));
            //Configure serilog

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{filename}-.txt", restrictedToMinimumLevel:Serilog.Events.LogEventLevel.Information
                ,outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz} [{Level:u3}] {NewLine} {Exception}",
                rollingInterval :RollingInterval.Day )
                .CreateLogger();
            //  LoggingServiceCollectionExtensions.AddLogging (services);

            //Add JWT authetication scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
            return services;
        }
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
         //   app.UseMiddleware<ListenToOnlyApiGateWay>();

            return app;
        }
    }
}
