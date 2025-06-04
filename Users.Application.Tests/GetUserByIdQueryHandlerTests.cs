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

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private GetUserByIdQueryHandler CreateHandler() => new(_userRepoMock.Object);

    [Fact(DisplayName = "نجاح جلب المستخدم عند وجوده")]
    public async Task GetUserById_Success_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "أحمد", LastName = "علي", Email = "a@a.com", PhoneNumber = "0123456789" };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(user);
        var handler = CreateHandler();
        var cmd = new GetUserByIdQuery(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب بيانات المستخدم بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.Id);
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
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
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
    }
} 