using Xunit;
using Moq;
using Users.Application.Commands.DeleteUser;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<DeleteUserCommandHandler>> _loggerMock = new();

    private DeleteUserCommandHandler CreateHandler() =>
        new(_userRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح الحذف عند صحة المعرف ووجود المستخدم")]
    public async Task DeleteUser_Success_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEntity = new User { Id = userId };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(userEntity);
        var handler = CreateHandler();
        var cmd = new DeleteUserCommand(userId);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم حذف المستخدم بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند معرف مستخدم غير صالح")]
    public async Task DeleteUser_Fails_WhenUserIdInvalid()
    {
        // Arrange
        var handler = CreateHandler();
        var cmd = new DeleteUserCommand(Guid.Empty);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("معرف المستخدم غير صالح", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند عدم وجود المستخدم")]
    public async Task DeleteUser_Fails_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حدوث استثناء داخلي")]
    public async Task DeleteUser_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new DeleteUserCommand(userId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في حذف المستخدم", result.Message);
    }
} 