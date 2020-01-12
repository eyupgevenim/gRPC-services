using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Grpc.Server.Extensions
{
    public static class ConfigureExtensions
    {
        public static IEndpointConventionBuilder AuthEndpointMap(this IEndpointRouteBuilder MapGet, string pattern, IConfiguration confg)
        {
            return MapGet.Map(pattern, context =>
            {
                var userName = context.Request.Query["username"];
                var password = context.Request.Query["password"];
                if (userName == confg.GetValue<string>("User:UserName") 
                && password == confg.GetValue<string>("User:Password"))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return context.Response.WriteAsync(GetJwtToken(confg));
                }

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return context.Response.WriteAsync("Bad Request");
            });
        }

        private static string GetJwtToken(IConfiguration confg)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confg.GetValue<string>("Jwt:IssuerSigningKey")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: confg.GetValue<string>("Jwt:ValidIssuer"),
                audience: confg.GetValue<string>("Jwt:ValidAudience"),
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(confg.GetValue<int>("Jwt:Expires")),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
