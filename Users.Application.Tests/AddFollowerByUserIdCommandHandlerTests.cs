using Xunit;
using Moq;
using Users.Application.Command.AddFollowerByUserId;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AddFollowerByUserIdCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IFollowerRepository> _followerRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<AddFollowerByUserIdCommandHandler>> _loggerMock = new();

    private AddFollowerByUserIdCommandHandler CreateHandler() =>
        new(_userRepoMock.Object, _followerRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح الإضافة عند صحة المعرفات ووجود المستخدمين وعدم وجود متابعة مسبقة")]
    public async Task AddFollower_Success_WhenValid()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        var followingUser = new User { Id = followingId, Followees = new List<Follower>() };
        var followerUser = new User { Id = followerId };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followingId)).ReturnsAsync(followingUser);
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followerId)).ReturnsAsync(followerUser);
        _followerRepoMock.Setup(r => r.AddAsync(It.IsAny<Follower>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم إضافة المتابع بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند معرف المتابِع غير صالح")]
    public async Task AddFollower_Fails_WhenFollowingIdInvalid()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(followerId, Guid.Empty);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("معرف المستخدم المتابِع غير صالح", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند معرف المتابَع غير صالح")]
    public async Task AddFollower_Fails_WhenFollowerIdInvalid()
    {
        // Arrange
        var followingId = Guid.NewGuid();
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(Guid.Empty, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("معرف المستخدم المتابَع غير صالح", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند محاولة المستخدم متابعة نفسه")]
    public async Task AddFollower_Fails_WhenUserFollowsSelf()
    {
        // Arrange
        var id = Guid.NewGuid();
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(id, id);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("لا يمكن للمستخدم متابعة نفسه", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند عدم وجود المستخدم المتابِع")]
    public async Task AddFollower_Fails_WhenFollowingUserNotFound()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followingId)).ReturnsAsync((User)null);
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followerId)).ReturnsAsync(new User { Id = followerId });
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم المتابِع غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند عدم وجود المستخدم المتابَع")]
    public async Task AddFollower_Fails_WhenFollowerUserNotFound()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        var followingUser = new User { Id = followingId, Followees = new List<Follower>() };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followingId)).ReturnsAsync(followingUser);
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followerId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم المتابَع غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند حدوث استثناء داخلي")]
    public async Task AddFollower_Fails_WhenExceptionThrown()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();
        var followingUser = new User { Id = followingId, Followees = new List<Follower>() };
        var followerUser = new User { Id = followerId };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followingId)).ReturnsAsync(followingUser);
        _userRepoMock.Setup(r => r.GetByIdWithDetails(followerId)).ReturnsAsync(followerUser);
        _followerRepoMock.Setup(r => r.AddAsync(It.IsAny<Follower>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new AddFollowerByUserIdCommand(followerId, followingId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في إضافة المتابع", result.Message);
    }
} 