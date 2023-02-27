using MassTransit;
using MediatR;
using Scribble.Blogs.Contracts.Events;
using Scribble.Blogs.Infrastructure.Data.Requests.Commands;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Commands;

public class UpdateBlogCommand : IRequest
{
    public UpdateBlogCommand(BlogEntity model) => Model = model;
    public BlogEntity Model { get; }
}

public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateBlogCommandHandler(IUnitOfWorkFactory factory, IPublishEndpoint publishEndpoint)
    {
        _factory = factory;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(UpdateBlogCommand request, CancellationToken token)
    {
        using var unitOfWork = await _factory.CreateAsync(true, token);

        await unitOfWork.ExecuteAsync(new UpdateBlogDbCommand(request.Model), token)
            .ConfigureAwait(false);

        await _publishEndpoint
            .Publish(new BlogEntityUpdatedContract(), token)
            .ConfigureAwait(false);
        
        unitOfWork.Commit();

        return Unit.Value;
    }
}