using Xunit;
using Moq;
using Users.Application.Queries.GetUserById;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Result;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUserRatingRepository> _userRatingRepoMock = new();
    private readonly Mock<IFollowerRepository> _followerRepoMock = new();
    private readonly Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock = new();
    private GetUserByIdQueryHandler CreateHandler() => new(_userRepoMock.Object, _userRatingRepoMock.Object, _followerRepoMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح جلب المستخدم عند وجوده")]
    public async Task GetUserById_Success_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "أحمد", LastName = "علي", Email = "a@a.com", PhoneNumber = "0923456789" };
        var userRating = new UserRating { UserId = userId, Rating = 4, NumOfReviews = 10 };
        
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(user);
        _userRatingRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(userRating);
        _followerRepoMock.Setup(r => r.GetByFollowerAndFollowingId(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Follower)null);
        var handler = CreateHandler();
        var cmd = new GetUserByIdQuery(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب بيانات المستخدم بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.Id);
        Assert.Equal(4, result.Data.Rating);
        Assert.Equal(10, result.Data.NumOfReviews);
        Assert.False(result.Data.IsFollowed);
    }

    [Fact(DisplayName = "فشل الجلب عند عدم وجود المستخدم")]
    public async Task GetUserById_Fails_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new GetUserByIdQuery(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم غير موجود", result.Message);
        Assert.Equal(ResultStatus.NotFound, result.ResultStatus);
    }

    [Fact(DisplayName = "فشل الجلب عند حدوث استثناء داخلي")]
    public async Task GetUserById_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new GetUserByIdQuery(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("حدث خطأ أثناء جلب بيانات المستخدم", result.Message);
        Assert.Equal(ResultStatus.Failed, result.ResultStatus);
    }
} 