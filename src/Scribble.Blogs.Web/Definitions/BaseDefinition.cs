using Calabonga.AspNetCore.AppDefinitions;
using MediatR;
using Scribble.Shared.Infrastructure.Extensions;

namespace Scribble.Blogs.Web.Definitions;

public class BaseDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddControllers();
        
        services.AddResponseCaching();
        services.AddMemoryCache();

        services.AddUnitOfWork();

        services.AddMediatR(typeof(Program));

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseResponseCaching();
    }
}