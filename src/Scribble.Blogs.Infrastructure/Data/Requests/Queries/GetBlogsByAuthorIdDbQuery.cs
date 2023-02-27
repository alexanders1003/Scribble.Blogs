using System.Data;
using Dapper;
using Scribble.Blogs.Infrastructure.Extensions;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure;

namespace Scribble.Blogs.Infrastructure.Data.Requests.Queries;

public class GetBlogsByAuthorIdDbQuery : IDbRequest<IReadOnlyCollection<BlogEntity>>
{
    private readonly object _parameters;
    private const string Query = """
         SELECT * FROM Blogs WHERE AuthorId = @AuthorId;
         """;

    public GetBlogsByAuthorIdDbQuery(object parameters) => _parameters = parameters;

    public async Task<IReadOnlyCollection<BlogEntity>> ExecuteAsync(IDbConnection connection, IDbTransaction? transaction,
        CancellationToken token = default)
    {
        var entities =  await connection.QueryAsync<BlogEntity>(Query, _parameters, transaction)
            .ConfigureAwait(false);

        return entities.AsReadOnly();
    }
}