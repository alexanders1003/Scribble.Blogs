using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Scribble.Blogs.Models;

namespace Scribble.Blogs.Tests;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture { OmitAutoProperties = true }
                .Customize(new AutoMoqCustomization { ConfigureMembers = false });
            
            fixture.Customize<BlogEntity>(composer => composer.WithAutoProperties());
            fixture.Register<IReadOnlyCollection<BlogEntity>>(() => new []
            {
                new BlogEntity(), new BlogEntity(), new BlogEntity()
            });

            return fixture;
        }) { }
}