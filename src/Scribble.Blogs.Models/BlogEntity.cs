using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Scribble.Shared.Models;

namespace Scribble.Blogs.Models;

public class BlogEntity : AuditableEntity
{
    [Required]
    public Guid AuthorId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = null!;
    
    [MaxLength(1000)]
    public string Description { get; set; } = null!;
}