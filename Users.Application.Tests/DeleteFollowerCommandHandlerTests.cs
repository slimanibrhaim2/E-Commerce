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

    [Fact(DisplayName = "نجاح الحذف عند وجود علاقة المتابعة وعدم حذفها مسبقًا")]
    public async Task DeleteFollower_Success_WhenFollowerRelationshipExistsAndNotDeleted()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        var followerEntity = new Follower 
        { 
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId,
            DeletedAt = null 
        };

        _followerRepoMock.Setup(r => r.GetByFollowerAndFollowingId(followerId, followingId))
            .ReturnsAsync(followerEntity);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId, followingId);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم حذف المتابع بنجاح", result.Message);
        Assert.NotNull(followerEntity.DeletedAt);
    }

    [Fact(DisplayName = "فشل الحذف عند عدم وجود علاقة المتابعة")]
    public async Task DeleteFollower_Fails_WhenFollowerRelationshipNotFound()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetByFollowerAndFollowingId(followerId, followingId))
            .ReturnsAsync((Follower)null);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("علاقة المتابعة غير موجودة", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حذف علاقة المتابعة مسبقًا")]
    public async Task DeleteFollower_Fails_WhenAlreadyDeleted()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        var followerEntity = new Follower 
        { 
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId,
            DeletedAt = DateTime.UtcNow 
        };

        _followerRepoMock.Setup(r => r.GetByFollowerAndFollowingId(followerId, followingId))
            .ReturnsAsync(followerEntity);
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("تم حذف علاقة المتابعة مسبقاً", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حدوث استثناء داخلي")]
    public async Task DeleteFollower_Fails_WhenExceptionThrown()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetByFollowerAndFollowingId(followerId, followingId))
            .ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new DeleteFollowerCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في حذف المتابع", result.Message);
    }
} 