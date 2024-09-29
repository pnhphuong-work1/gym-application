using System.Text;
using GymApplication.Services.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GymApplication.api.Extension;

public static class JwtExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var jwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            
            // Save the token in the AuthenticationProperties
            // Storing the JWT in the AuthenticationProperties allows you to retrieve it from elsewhere within your application.
            // public async Task<IActionResult> SomeAction()
            // {
            //     // using Microsoft.AspNetCore.Authentication;
            //     var accessToken = await HttpContext.GetTokenAsync("access_token");
            //     // ...
            // }
            o.SaveToken = true;
            
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
            
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
            
            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                    }
                    
                    return Task.CompletedTask;
                }
            };
        });
        
        services.AddAuthorization();
    }
}