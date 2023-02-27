namespace Scribble.Blogs.Web.Definitions.MessageBroker;

public class MessageBrokerHostOptions
{
    public string Host { get; set; } = null!;
    public string VirtualHost { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}