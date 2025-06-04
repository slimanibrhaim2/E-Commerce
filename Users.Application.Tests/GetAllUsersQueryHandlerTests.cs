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

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private GetAllUsersQueryHandler CreateHandler() => new(_userRepoMock.Object);

    [Fact(DisplayName = "نجاح جلب المستخدمين عند وجود بيانات")]
    public async Task GetAllUsers_Success_WhenUsersExist()
    {
        // Arrange
        var users = new List<User> { new User { Id = Guid.NewGuid(), FirstName = "أحمد", LastName = "علي", Email = "a@a.com", PhoneNumber = "0123456789" } };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        var handler = CreateHandler();
        var cmd = new GetAllUsersQuery(new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم استرجاع المستخدمين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Data.Any());
    }

    [Fact(DisplayName = "نجاح جلب المستخدمين عند عدم وجود بيانات (قائمة فارغة)")]
    public async Task GetAllUsers_Success_WhenNoUsersExist()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        var handler = CreateHandler();
        var cmd = new GetAllUsersQuery(new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم استرجاع المستخدمين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Data);
    }

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
        Assert.Contains("DB error", result.Message);
    }
} 