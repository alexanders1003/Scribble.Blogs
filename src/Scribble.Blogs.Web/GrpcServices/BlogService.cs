using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Scribble.Blogs.Contracts.Proto;
using Scribble.Blogs.Infrastructure.Features.Commands;
using Scribble.Blogs.Infrastructure.Features.Queries;
using Scribble.Blogs.Models;

namespace Scribble.Blogs.Web.GrpcServices;

public class BlogService : BlogProtoService.BlogProtoServiceBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public BlogService(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public override async Task<BlogModel?> GetById(GetByIdGrpcRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var result)) return default;
        
        var entity = await _mediator.Send(new GetBlogByIdQuery(result), context.CancellationToken)
            .ConfigureAwait(false);

        return _mapper.Map<BlogModel>(entity);
    }

    public override async Task<GetPagedBlogsGrpcResponse> GetPagedBlogs(GetPagedBlogsGrpcRequest request, ServerCallContext context)
    {
        var entities = await _mediator
            .Send(new GetPagedBlogsQuery(request.PageIndex, request.PageSize), context.CancellationToken)
            .ConfigureAwait(false);

        var models = _mapper.Map<IReadOnlyCollection<BlogModel>>(entities);

        return new GetPagedBlogsGrpcResponse { Models = { models } };
    }

    public override async Task<GetBlogsByAuthorIdResponse> GetBlogsByAuthorId(GetBlogsByAuthorIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.AuthorId, out var result))
            return new GetBlogsByAuthorIdResponse();

        var entities = await _mediator
            .Send(new GetBlogsByAuthorIdQuery(result), context.CancellationToken)
            .ConfigureAwait(false);

        var models = _mapper.Map<IReadOnlyCollection<BlogModel>>(entities);

        return new GetBlogsByAuthorIdResponse { Models = { models } };
    }

    public override async Task<CreateGrpcResponse> Create(CreateGrpcRequest request, ServerCallContext context)
    {
        var entity = _mapper.Map<BlogEntity>(request.Model);

        var id = await _mediator.Send(new CreateBlogCommand(entity), context.CancellationToken)
            .ConfigureAwait(false);

        return new CreateGrpcResponse { Id = id.ToString() };
    }

    public override async Task<Empty> Update(UpdateGrpcRequest request, ServerCallContext context)
    {
        var entity = _mapper.Map<BlogEntity>(request.Model);

        await _mediator.Send(new UpdateBlogCommand(entity), context.CancellationToken)
            .ConfigureAwait(false);

        return new Empty();
    }

    public override async Task<Empty> Delete(DeleteGrpcRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var result)) return new Empty();

        await _mediator.Send(new DeleteBlogCommand(result), context.CancellationToken)
            .ConfigureAwait(false);

        return new Empty();
    }
    
    public override async Task<ExistsGrpcResponse> Exists(ExistsGrpcRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out _))
            return new ExistsGrpcResponse { Status = false };

        var entity = await GetById(new GetByIdGrpcRequest { Id = request.Id }, context)
            .ConfigureAwait(false);

        return new ExistsGrpcResponse { Status = entity is not null };
    }
}