using Xunit;
using Moq;
using Users.Application.Queries.GetAllFollowersByUserId;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Pagination;
using Core.Result;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class GetAllFollowersByUserIdQueryHandlerTests
{
    private readonly Mock<IFollowerRepository> _followerRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private GetAllFollowersByUserIdQueryHandler CreateHandler() => new(_followerRepoMock.Object, _userRepoMock.Object);

    [Fact(DisplayName = "نجاح جلب المتابعين عند وجود بيانات")]
    public async Task GetAllFollowers_Success_WhenFollowersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var followers = new List<Follower> { new Follower { Id = Guid.NewGuid(), FollowerId = Guid.NewGuid(), FollowingId = userId } };
        _followerRepoMock.Setup(r => r.GetFollowersByUserId(userId)).ReturnsAsync(followers);
        var handler = CreateHandler();
        var cmd = new GetAllFollowersByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب جميع المتابعين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Data.Any());
    }

    [Fact(DisplayName = "نجاح جلب المتابعين عند عدم وجود متابعين والمستخدم موجود")]
    public async Task GetAllFollowers_Success_WhenNoFollowersButUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetFollowersByUserId(userId)).ReturnsAsync(new List<Follower>());
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(new User { Id = userId });
        var handler = CreateHandler();
        var cmd = new GetAllFollowersByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب جميع المتابعين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Data);
    }

    [Fact(DisplayName = "فشل الجلب عند عدم وجود المستخدم")]
    public async Task GetAllFollowers_Fails_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetFollowersByUserId(userId)).ReturnsAsync(new List<Follower>());
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new GetAllFollowersByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الجلب عند حدوث استثناء داخلي")]
    public async Task GetAllFollowers_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _followerRepoMock.Setup(r => r.GetFollowersByUserId(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new GetAllFollowersByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("حدث خطأ أثناء جلب قائمة المتابعين", result.Message);
    }
} 