using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scribble.Blogs.Infrastructure.Features.Commands;
using Scribble.Blogs.Infrastructure.Features.Queries;
using Scribble.Blogs.Models;

namespace Scribble.Blogs.Web.Controllers;

[ApiController]
[Route("api/v1/blogs")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BlogsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BlogsController(IMediator mediator)
        => _mediator = mediator;

    [HttpGet("{id:guid}"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BlogEntity), StatusCodes.Status200OK)]
    public async Task<ActionResult<BlogEntity?>> GetBlogByIdAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) 
            return new BadRequestResult();
        
        var entity = await _mediator.Send(new GetBlogByIdQuery(id), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (entity is null)
            return new NotFoundResult();

        return new OkObjectResult(entity);
    }

    [HttpGet, AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IReadOnlyCollection<BlogEntity>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BlogEntity>>> GetPagedBlogsAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 0 || pageSize < 0)
            return new BadRequestResult();

        var collection = await _mediator.Send(new GetPagedBlogsQuery(pageIndex, pageSize), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new OkObjectResult(collection);
    }

    [HttpGet, AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<BlogEntity>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BlogEntity>?>> GetBlogsByAuthorIdAsync(Guid authorId)
    {
        if (authorId.Equals(Guid.Empty))
            return new BadRequestResult();
        
        var collection = await _mediator.Send(new GetBlogsByAuthorIdQuery(authorId), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (collection is not null)
            return new OkObjectResult(collection);
        return new NotFoundResult();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> CreateBlogAsync(BlogEntity model)
    {
        if (!ModelState.IsValid)
            return new BadRequestObjectResult(ModelState);
        
        var id = await _mediator.Send(new CreateBlogCommand(model), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new OkObjectResult(id);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UpdateBlogAsync(BlogEntity model)
    {
        if (!ModelState.IsValid)
            return new BadRequestObjectResult(ModelState);
        
        await _mediator.Send(new UpdateBlogCommand(model), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new OkResult();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteBlogAsync(Guid id)
    {
        if (id.Equals(Guid.Empty))
            return new BadRequestResult();
        
        await _mediator.Send(new DeleteBlogCommand(id), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new OkResult();
    }
}