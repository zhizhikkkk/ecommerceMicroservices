using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class JWTAuthentificationScheme
    {
        public static IServiceCollection AddJWTAuthentificationScheme(this IServiceCollection services, IConfiguration config)
        {
            //add JWT service

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    var key = Encoding.UTF8.GetBytes(config.GetSection("Authentification:Key").Value!);
                    string issuer = config.GetSection("Authentification:Issuer").Value!;
                    string audience = config.GetSection("Authentification:Audience").Value!;

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience= true,
                        ValidateLifetime= false,
                        ValidateIssuerSigningKey= true,
                        ValidIssuer = issuer,
                        ValidAudience=audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            return services;
        }
    }
}
