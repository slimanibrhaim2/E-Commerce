using Xunit;
using Moq;
using Users.Application.Commands.UpdateUser;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.Command.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock = new();

    private UpdateUserCommandHandler CreateHandler() =>
        new(_userRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح التحديث عند صحة البيانات")]
    public async Task UpdateUser_Success_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEntity = new User { Id = userId, Email = "old@example.com" };
        var dto = new CreateUserDTO
        {
            FirstName = "محمد",
            LastName = "سعيد",
            Email = "new@example.com",
            PhoneNumber = "0123456789"
        };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(userEntity);
        _userRepoMock.Setup(r => r.GetByEmail(dto.Email)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم تحديث بيانات المستخدم بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند معرف مستخدم غير صالح")]
    public async Task UpdateUser_Fails_WhenUserIdInvalid()
    {
        // Arrange
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "new@example.com", PhoneNumber = "0123456789" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(Guid.Empty, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("معرف المستخدم غير صالح", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند نقص الاسم الأول")]
    public async Task UpdateUser_Fails_WhenFirstNameMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = null, LastName = "سعيد", Email = "new@example.com", PhoneNumber = "0123456789" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("الاسم الأول مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند نقص الاسم الأخير")]
    public async Task UpdateUser_Fails_WhenLastNameMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = null, Email = "new@example.com", PhoneNumber = "0123456789" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("الاسم الأخير مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند نقص البريد الإلكتروني")]
    public async Task UpdateUser_Fails_WhenEmailMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = null, PhoneNumber = "0123456789" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("البريد الإلكتروني مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند نقص رقم الهاتف")]
    public async Task UpdateUser_Fails_WhenPhoneNumberMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "new@example.com", PhoneNumber = null };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("رقم الهاتف مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند صيغة بريد إلكتروني غير صحيحة")]
    public async Task UpdateUser_Fails_WhenEmailFormatInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "invalid-email", PhoneNumber = "0123456789" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("صيغة البريد الإلكتروني غير صحيحة", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند صيغة رقم هاتف غير صحيحة")]
    public async Task UpdateUser_Fails_WhenPhoneNumberFormatInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "new@example.com", PhoneNumber = "123" };
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("صيغة رقم الهاتف غير صحيحة", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند عدم وجود المستخدم")]
    public async Task UpdateUser_Fails_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "new@example.com", PhoneNumber = "0123456789" };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند وجود بريد إلكتروني مستخدم مسبقًا")]
    public async Task UpdateUser_Fails_WhenEmailAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEntity = new User { Id = userId, Email = "old@example.com" };
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "used@example.com", PhoneNumber = "0123456789" };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ReturnsAsync(userEntity);
        _userRepoMock.Setup(r => r.GetByEmail(dto.Email)).ReturnsAsync(new User());
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("مستخدم بالفعل", result.Message);
    }

    [Fact(DisplayName = "فشل التحديث عند حدوث استثناء داخلي")]
    public async Task UpdateUser_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateUserDTO { FirstName = "محمد", LastName = "سعيد", Email = "new@example.com", PhoneNumber = "0123456789" };
        _userRepoMock.Setup(r => r.GetByIdWithDetails(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new UpdateUserCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في تحديث بيانات المستخدم", result.Message);
    }
} 