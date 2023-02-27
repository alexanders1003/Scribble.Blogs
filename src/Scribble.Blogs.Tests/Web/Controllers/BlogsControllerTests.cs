using AutoFixture.Xunit2;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Scribble.Blogs.Models;
using Scribble.Blogs.Web.Controllers;
using Xunit;

namespace Scribble.Blogs.Tests.Web.Controllers;

public class BlogsControllerTests
{
    [Theory, AutoMoqData]
    public async Task GetBlogByIdAsync_WhenIdExists_ReturnsStatusCode200([Frozen] Mock<IMediator> mediator, BlogEntity entity)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<BlogEntity?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogByIdAsync(entity.Id);

        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
        Assert.Equal(entity, actionResult.Value);
    }

    [Theory, AutoMoqData]
    public async Task GetBlogByIdAsync_WhenIdNotExists_ReturnsStatusCode404([Frozen] Mock<IMediator> mediator)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<BlogEntity?>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<BlogEntity>(null!)!);
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogByIdAsync(Guid.NewGuid());

        var actionResult = Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task GetBlogByIdAsync_WhenIdIsEmpty_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator)
    {
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogByIdAsync(Guid.Empty);

        var actionResult = Assert.IsType<BadRequestResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task GetPagedBlogsAsync_WhenParametersAreCorrect_ReturnsStatusCode200([Frozen] Mock<IMediator> mediator, IReadOnlyCollection<BlogEntity> collection)
    {
        const int pageIndex = 0;
        const int pageSize = 5;

        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<IReadOnlyCollection<BlogEntity>>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(collection));

        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetPagedBlogsAsync(pageIndex, pageSize);

        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
        Assert.Equal(collection, actionResult.Value);
    }
    
    [Theory, AutoMoqData]
    public async Task GetPagedBlogsAsync_WhenParametersAreNotCorrect_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator, IReadOnlyCollection<BlogEntity> collection)
    {
        const int pageIndex = -5;
        const int pageSize = 5;

        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<IReadOnlyCollection<BlogEntity>>>(), CancellationToken.None))
            .Returns(Task.FromResult(collection));

        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetPagedBlogsAsync(pageIndex, pageSize);

        var actionResult = Assert.IsType<BadRequestResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }

    [Theory, AutoMoqData]
    public async Task GetBlogsByAuthorIdAsync_WhenAuthorIdExists_ReturnsStatusCode200([Frozen] Mock<IMediator> mediator,
        IReadOnlyCollection<BlogEntity> collection)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<IReadOnlyCollection<BlogEntity>>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(collection));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogsByAuthorIdAsync(Guid.NewGuid());

        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode); 
    }
    
    [Theory, AutoMoqData]
    public async Task GetBlogsByAuthorIdAsync_WhenAuthorIdIsEmpty_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator,
        IReadOnlyCollection<BlogEntity> collection)
    {
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogsByAuthorIdAsync(Guid.Empty);

        var actionResult = Assert.IsType<BadRequestResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task GetBlogsByAuthorIdAsync_WhenAuthorIdNotExist_ReturnsStatusCode404([Frozen] Mock<IMediator> mediator,
        IReadOnlyCollection<BlogEntity> collection)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<IReadOnlyCollection<BlogEntity>>>(), CancellationToken.None))
            .Returns(Task.FromResult<IReadOnlyCollection<BlogEntity>>(null!));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.GetBlogsByAuthorIdAsync(Guid.NewGuid());

        var actionResult = Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, actionResult.StatusCode);
    }

    [Theory, AutoMoqData]
    public async Task CreateBlogAsync_WhenModelIsValid_ReturnsStatusCode200([Frozen] Mock<IMediator> mediator,
        BlogEntity entity)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(entity.Id));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var result = await controller.CreateBlogAsync(entity);

        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task CreateBlogAsync_WhenModelIsNotValid_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator,
        BlogEntity entity)
    {
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        entity.Title = null!;
        controller.ModelState.AddModelError("Title", "Required");
        
        var result = await controller.CreateBlogAsync(entity);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateBlogAsync_WhenModelIsValid_ReturnsStatusCode200([Frozen] Mock<IMediator> mediator,
        BlogEntity entity)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest>(), CancellationToken.None))
            .Returns(Task.FromResult(Unit.Value));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.UpdateBlogAsync(entity);

        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateBlogAsync_WhenModelIsNotValid_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator,
        BlogEntity entity)
    {
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        entity.Title = null!;
        controller.ModelState.AddModelError("Title", "Required");
        
        var result = await controller.UpdateBlogAsync(entity);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task DeleteBlogAsync_WhenModelIsValid_ReturnsStatusCode404([Frozen] Mock<IMediator> mediator)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest>(), CancellationToken.None))
            .Returns(Task.FromResult(Unit.Value));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.DeleteBlogAsync(Guid.NewGuid());

        var actionResult = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, actionResult.StatusCode);
    }
    
    [Theory, AutoMoqData]
    public async Task DeleteBlogAsync_WhenModelIsNotValid_ReturnsStatusCode400([Frozen] Mock<IMediator> mediator, 
        BlogEntity entity)
    {
        mediator
            .Setup(x => x.Send(It.IsAny<IRequest>(), CancellationToken.None))
            .Returns(Task.FromResult(Unit.Value));
        
        var controller = new BlogsController(mediator.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.DeleteBlogAsync(Guid.Empty);

        var actionResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, actionResult.StatusCode);
    }
}