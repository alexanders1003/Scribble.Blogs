using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Scribble.Blogs.Web.Definitions.Identity;

public class IdentityDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        var authSection = builder.Configuration.GetSection("Authentication");

        var authority = authSection.GetValue<string>("ServerUrl");
        var issuer = authSection.GetValue<string>("Issuer");
        var audience = authSection.GetValue<string>("Audience");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;

                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidateIssuer = true,

                    ValidAudience = audience,
                    ValidateAudience = true,

                    ValidateLifetime = true,
                };
            });
    }
    
    public override void ConfigureApplication(WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}