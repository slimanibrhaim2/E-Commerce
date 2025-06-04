using Xunit;
using Moq;
using Users.Application.Commands.DeleteFollower;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DeleteFollowerCommandHandlerTests
{
    private readonly Mock<IFollowerRepository> _followerRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<DeleteFollowerCommandHandler>> _loggerMock = new();

    private DeleteFollowerCommandHandler CreateHandler() =>
        new(_followerRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح الحذف عند وجود المتابع وعدم حذفه مسبقًا")]
    public async Task DeleteFollower_Success_WhenFollowerExistsAndNotDeleted()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followerEntity = new Follower { Id = followerId, DeletedAt = null };
        _followerRepoMock.Setup(r => r.GetByIdAsync(followerId)).ReturnsAsync(followerEntity);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم حذف المتابع بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند عدم وجود المتابع")]
    public async Task DeleteFollower_Fails_WhenFollowerNotFound()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetByIdAsync(followerId)).ReturnsAsync((Follower)null);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المتابع غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حذف المتابع مسبقًا")]
    public async Task DeleteFollower_Fails_WhenAlreadyDeleted()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followerEntity = new Follower { Id = followerId, DeletedAt = DateTime.UtcNow };
        _followerRepoMock.Setup(r => r.GetByIdAsync(followerId)).ReturnsAsync(followerEntity);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المتابع محذوف بالفعل", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حدوث استثناء داخلي")]
    public async Task DeleteFollower_Fails_WhenExceptionThrown()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetByIdAsync(followerId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في حذف المتابع", result.Message);
    }
} 