using System.Reflection;
using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.OpenApi.Models;

namespace Scribble.Blogs.Web.Definitions.Documentation;

public class SwaggerDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("scribble.blogs.v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Scribble.Blogs Microservice",
                Description = "Web API for Blog CRUD operations"
            });

        });
    }

    public override void ConfigureApplication(WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }
}