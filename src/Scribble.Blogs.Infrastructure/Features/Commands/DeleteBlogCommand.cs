using MassTransit;
using MediatR;
using Scribble.Blogs.Contracts.Events;
using Scribble.Blogs.Infrastructure.Data.Requests.Commands;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Commands;

public class DeleteBlogCommand : IRequest
{
    public DeleteBlogCommand(Guid blogId) => BlogId = blogId;
    public Guid BlogId { get; }
}

public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteBlogCommandHandler(IUnitOfWorkFactory factory, IPublishEndpoint publishEndpoint)
    {
        _factory = factory;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        using var unitOfWork = await _factory.CreateAsync(true, cancellationToken);

        await unitOfWork.ExecuteAsync(new DeleteBlogDbCommand(request.BlogId), cancellationToken)
            .ConfigureAwait(false);

        await _publishEndpoint
            .Publish(new BlogEntityDeletedContract { Id = request.BlogId }, cancellationToken)
            .ConfigureAwait(false);
        
        unitOfWork.Commit();
        
        return Unit.Value;
    }
}