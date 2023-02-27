using MassTransit;
using MediatR;
using Scribble.Blogs.Contracts.Events;
using Scribble.Blogs.Infrastructure.Data.Requests.Commands;
using Scribble.Blogs.Infrastructure.Data.Requests.Queries;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Commands;

public class CreateBlogCommand : IRequest<Guid>
{
    public CreateBlogCommand(BlogEntity model) => Model = model;
    public BlogEntity Model { get; }
}

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Guid>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateBlogCommandHandler(IUnitOfWorkFactory factory, IPublishEndpoint publishEndpoint)
    {
        _factory = factory;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateBlogCommand request, CancellationToken token)
    {
        using var unitOfWork = await _factory.CreateAsync(true, token);

        var blogId = await unitOfWork
            .ExecuteAsync(new CreateBlogDbCommand(request.Model), token)
            .ConfigureAwait(false);

        await _publishEndpoint
            .Publish(new BlogEntityCreatedContract { Id = blogId }, token)
            .ConfigureAwait(false);
        
        unitOfWork.Commit();

        return blogId;
    }
}