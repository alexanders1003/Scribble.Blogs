using System.Data;
using Dapper;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure;

namespace Scribble.Blogs.Infrastructure.Data.Requests.Queries;

public class GetBlogByIdDbQuery : IDbRequest<BlogEntity>
{
    private readonly object _parameters;
    private const string Query = """
          SELECT * FROM Blogs WHERE Id = @Id;
          """;

    public GetBlogByIdDbQuery(object parameters)
        => _parameters = parameters;

    public async Task<BlogEntity> ExecuteAsync(IDbConnection connection, IDbTransaction? transaction, CancellationToken token = default)
    {
        return await connection.QuerySingleAsync<BlogEntity>(Query, _parameters, transaction)
            .ConfigureAwait(false);
    }
}