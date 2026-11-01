using CleanArchietureNoFrontEnd.Application.Common.Interfaces;
using CleanArchietureNoFrontEnd.Application.Common.Middleware;
using CleanArchietureNoFrontEnd.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wolverine;

namespace CleanArchietureNoFrontEnd.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var envelope = new Envelope(new CreateTodoItemCommand { ListId = 1, Title = "title" });

        await LoggingMiddleware.Before(envelope, _user.Object, _identityService.Object, NullLoggerFactory.Instance);

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var envelope = new Envelope(new CreateTodoItemCommand { ListId = 1, Title = "title" });

        await LoggingMiddleware.Before(envelope, _user.Object, _identityService.Object, NullLoggerFactory.Instance);

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
