namespace NoteBook.Web.Configuration;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

/// <summary>
/// JWT Authentication configuration
/// Handles JWT token validation and authentication scheme setup
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Configure JWT Authentication with token validation parameters
    /// </summary>
    public static void AddJwtAuthentication(WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret") 
            ?? "your-super-secret-key-that-is-very-long-and-secure";
        var issuer = jwtSettings.GetValue<string>("Issuer") ?? "NoteBook";
        var audience = jwtSettings.GetValue<string>("Audience") ?? "NoteBookUsers";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
        });
    }
}
