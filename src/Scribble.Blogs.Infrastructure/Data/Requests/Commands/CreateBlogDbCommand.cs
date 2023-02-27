using System.Data;
using Dapper;
using Scribble.Shared.Infrastructure;

namespace Scribble.Blogs.Infrastructure.Data.Requests.Commands;

public class CreateBlogDbCommand : IDbRequest<Guid>
{
    private readonly object _parameters;
    private const string Query = """
          INSERT INTO Blogs (AuthorId, Title, Description)
          VALUES (@AuthorId, @Title, @Description)
          SELECT CAST(SCOPE_IDENTITY() as Guid);
          """;

    public CreateBlogDbCommand(object parameters)
        => _parameters = parameters;

    public async Task<Guid> ExecuteAsync(IDbConnection connection, IDbTransaction? transaction, CancellationToken token)
    {
        return await connection.QuerySingleAsync<Guid>(Query, _parameters, transaction)
            .ConfigureAwait(false);
    }
}