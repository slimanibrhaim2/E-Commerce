using Xunit;
using Moq;
using Users.Application.Queries.GetAllUsers;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Pagination;
using Core.Result;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUserRatingRepository> _userRatingRepoMock = new();
    private readonly Mock<ILogger<GetAllUsersQueryHandler>> _loggerMock = new();
    private GetAllUsersQueryHandler CreateHandler() => new(_userRepoMock.Object, _userRatingRepoMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح جلب المستخدمين عند وجود بيانات")]
    public async Task GetAllUsers_Success_WhenUsersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<User> { new User { Id = userId, FirstName = "أحمد", LastName = "علي", Email = "a@a.com", PhoneNumber = "0923456789" } };
        var userRatings = new List<UserRating> { new UserRating { UserId = userId, Rating = 4, NumOfReviews = 5 } };
        
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        _userRatingRepoMock.Setup(r => r.GetByUserIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(userRatings);
        var handler = CreateHandler();
        var cmd = new GetAllUsersQuery(new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب المستخدمين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Data.Any());
        Assert.Equal(4, result.Data.Data.First().Rating);
        Assert.Equal(5, result.Data.Data.First().NumOfReviews);
    }

//    [Fact(DisplayName = "نجاح جلب المستخدمين عند عدم وجود بيانات (قائمة فارغة)")]
//    public async Task GetAllUsers_Success_WhenNoUsersExist()
//    {
//        // Arrange
//        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
//        var handler = CreateHandler();
//        var cmd = new GetAllUsersQuery(new Core.Pagination.PaginationParameters());

//        // Act
//        var result = await handler.Handle(cmd, CancellationToken.None);

//        // Assert
//        Assert.True(result.Success);
//        Assert.Equal("تم استرجاع المستخدمين بنجاح", result.Message);
//        Assert.NotNull(result.Data);
//        Assert.Empty(result.Data.Data);
//    }

    [Fact(DisplayName = "فشل الجلب عند حدوث استثناء داخلي")]
    public async Task GetAllUsers_Fails_WhenExceptionThrown()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new GetAllUsersQuery(new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("حدث خطأ أثناء جلب المستخدمين", result.Message);
        Assert.Equal(ResultStatus.Failed, result.ResultStatus);
    }
} 