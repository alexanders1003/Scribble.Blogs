using AutoMapper;
using Scribble.Blogs.Contracts.Proto;
using Scribble.Blogs.Models;

namespace Scribble.Blogs.Web.Definitions.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BlogEntity, BlogModel>()
            .ReverseMap();
    }
}