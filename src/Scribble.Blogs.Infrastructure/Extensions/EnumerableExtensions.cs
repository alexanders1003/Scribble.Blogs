using System.Collections.ObjectModel;

namespace Scribble.Blogs.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    public static IReadOnlyList<TSource> AsReadOnly<TSource>(this IEnumerable<TSource> enumerable)
        => new ReadOnlyCollection<TSource>(enumerable.ToList());
}