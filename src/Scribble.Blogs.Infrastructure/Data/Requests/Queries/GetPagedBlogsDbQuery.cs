using System.Collections.ObjectModel;
using System.Data;
using Dapper;
using Scribble.Blogs.Models;
using Scribble.Shared.Infrastructure;

namespace Scribble.Blogs.Infrastructure.Data.Requests.Queries;

public class GetPagedBlogsDbQuery : IDbRequest<IReadOnlyCollection<BlogEntity>>
{
    private readonly object _parameters;
    private const string Query = """
          SELECT * FROM Blogs LIMIT @PageSize OFFSET @PageIndex;
          """;

    public GetPagedBlogsDbQuery(object parameters) => _parameters = parameters;

    public async Task<IReadOnlyCollection<BlogEntity>> ExecuteAsync(IDbConnection connection, IDbTransaction? transaction,
        CancellationToken token = default)
    {
        var entities = await connection.QueryAsync<BlogEntity>(Query, _parameters, transaction)
            .ConfigureAwait(false);
        
        return entities is not null 
            ? new ReadOnlyCollection<BlogEntity>(entities.ToList()) 
            : new ReadOnlyCollection<BlogEntity>(new List<BlogEntity>());
    }
}