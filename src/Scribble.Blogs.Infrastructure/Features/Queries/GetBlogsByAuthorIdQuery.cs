using MediatR;
using Scribble.Blogs.Infrastructure.Data.Requests.Queries;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Queries;

public class GetBlogsByAuthorIdQuery : IRequest<IReadOnlyCollection<BlogEntity>?>
{
    public GetBlogsByAuthorIdQuery(Guid authorId) => AuthorId = authorId;
    public Guid AuthorId { get; }
}

public class GetBlogsByAuthorIdQueryHandler : IRequestHandler<GetBlogsByAuthorIdQuery, IReadOnlyCollection<BlogEntity>?>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetBlogsByAuthorIdQueryHandler(IUnitOfWorkFactory factory) => _factory = factory;

    public async Task<IReadOnlyCollection<BlogEntity>?> Handle(GetBlogsByAuthorIdQuery request, CancellationToken token)
    {
        using var unitOfWork = await _factory.CreateAsync(false, token);

        return await unitOfWork.ExecuteAsync(new GetBlogsByAuthorIdDbQuery(request.AuthorId), token)
            .ConfigureAwait(false);
    }
}