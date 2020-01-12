using Grpc.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server.Extensions
{
    public static class ConfigureServiceExtensions
    {
        /// <summary>
        /// Add JwtBearer Authentication configures
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration confg)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = confg.GetValue<string>("Jwt:ValidIssuer"),
                        ValidAudience = confg.GetValue<string>("Jwt:ValidAudience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confg.GetValue<string>("Jwt:IssuerSigningKey")))
                    };
                });

            return services;
        }

        public static IServiceCollection AddDataDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IRepository<VendorProductEntity>>(new Repository<VendorProductEntity>(DbData.VendorProductData));

            services.AddSingleton<IRepository<OrderEntity>>(new Repository<OrderEntity>(new List<OrderEntity>()));

            return services;
        }

    }
}
