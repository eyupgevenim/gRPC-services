using Grpc.Server.Extensions;
using Grpc.Server.GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Grpc.Server
{
    public class Startup
    {
        public IConfiguration configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataDependencyInjection();
            services.AddJwtBearerAuthentication(configuration);
            services.AddAuthorization();
            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<CatalogService>();
                endpoints.MapGrpcService<OrderingService>();
                endpoints.AuthEndpointMap("/auth/token", configuration);
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client." +
                        " To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

    }
}
