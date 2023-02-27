using MediatR;
using Scribble.Blogs.Infrastructure.Data.Requests.Queries;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure.Factories;

namespace Scribble.Blogs.Infrastructure.Features.Queries;

public class GetPagedBlogsQuery : IRequest<IReadOnlyCollection<BlogEntity>>
{
    public GetPagedBlogsQuery(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
    
    public int PageIndex { get; }
    public int PageSize { get; }
}

public class GetPagedBlogsQueryHandler : IRequestHandler<GetPagedBlogsQuery, IReadOnlyCollection<BlogEntity>>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetPagedBlogsQueryHandler(IUnitOfWorkFactory factory) => _factory = factory;

    public async Task<IReadOnlyCollection<BlogEntity>> Handle(GetPagedBlogsQuery request, CancellationToken token)
    {
        using var unitOfWork = await _factory.CreateAsync(false, token)
            .ConfigureAwait(false);

        var collection = await unitOfWork.ExecuteAsync(new GetPagedBlogsDbQuery(new
        {
            request.PageIndex, request.PageSize
        }), token);

        return collection!;
    }
}