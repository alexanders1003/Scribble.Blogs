using MediatR;
using Scribble.Blogs.Infrastructure.Data.Requests.Queries;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Queries;

public class GetBlogByIdQuery : IRequest<BlogEntity?>
{
    public GetBlogByIdQuery(Guid blogId) => BlogId = blogId;
    public Guid BlogId { get; }
}

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, BlogEntity?>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetBlogByIdQueryHandler(IUnitOfWorkFactory factory) => _factory = factory;

    public async Task<BlogEntity?> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        using var unitOfWork = await _factory.CreateAsync(false, cancellationToken);

        return await unitOfWork.ExecuteAsync(new GetBlogByIdDbQuery(request.BlogId), cancellationToken)
            .ConfigureAwait(false);
    }
}