using System.Data;
using Dapper;
using Scribble.Shared.Infrastructure;

namespace Scribble.Blogs.Infrastructure.Data.Requests.Commands;

public class UpdateBlogDbCommand : IDbRequest
{
    private readonly object _parameters;
    private const string Query = """
          UPDATE Blogs
          SET Title = @Title, Content = @Content, Description = @Description
          WHERE Id = @Id;
          """;

    public UpdateBlogDbCommand(object parameters)
        => _parameters = parameters;

    public async Task ExecuteAsync(IDbConnection connection, IDbTransaction? transaction,
        CancellationToken token = default)
    {
        await connection.ExecuteAsync(Query, _parameters, transaction)
            .ConfigureAwait(false);
    }
}