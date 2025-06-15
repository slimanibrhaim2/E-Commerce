using Xunit;
using Moq;
using Users.Application.Commands.CreateUser;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock = new();

    private CreateUserCommandHandler CreateHandler() =>
        new(_userRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح إنشاء مستخدم عند إدخال بيانات صحيحة")]
    public async Task CreateUser_Success_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = "0923456789"
        };
        _userRepoMock.Setup(r => r.GetByEmail(dto.Email)).ReturnsAsync((User)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم إنشاء المستخدم بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند نقص الاسم الأول")]
    public async Task CreateUser_Fails_WhenFirstNameMissing()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = null,
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = "0123456789"
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("الاسم الأول مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند نقص الاسم الأخير")]
    public async Task CreateUser_Fails_WhenLastNameMissing()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = null,
            Email = "ahmed@example.com",
            PhoneNumber = "0123456789"
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("الاسم الأخير مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند نقص البريد الإلكتروني")]
    public async Task CreateUser_Fails_WhenEmailMissing()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = null,
            PhoneNumber = "0123456789"
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("البريد الإلكتروني مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند نقص رقم الهاتف")]
    public async Task CreateUser_Fails_WhenPhoneNumberMissing()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = null
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("رقم الهاتف مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند وجود بريد إلكتروني مستخدم مسبقًا")]
    public async Task CreateUser_Fails_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = "0923456789"
        };
        _userRepoMock.Setup(r => r.GetByEmail(dto.Email)).ReturnsAsync(new User());
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("مستخدم بالفعل", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند صيغة بريد إلكتروني غير صحيحة")]
    public async Task CreateUser_Fails_WhenEmailFormatInvalid()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed.com",
            PhoneNumber = "0123456789"
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("صيغة البريد الإلكتروني غير صحيحة", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند صيغة رقم هاتف غير صحيحة")]
    public async Task CreateUser_Fails_WhenPhoneNumberFormatInvalid()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = "123"
        };
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("صيغة رقم الهاتف غير صحيحة", result.Message);
    }

    [Fact(DisplayName = "فشل الإنشاء عند حدوث استثناء داخلي")]
    public async Task CreateUser_Fails_WhenExceptionThrown()
    {
        // Arrange
        var dto = new CreateUserDTO
        {
            FirstName = "أحمد",
            LastName = "علي",
            Email = "ahmed@example.com",
            PhoneNumber = "0923456789"
        };
        _userRepoMock.Setup(r => r.GetByEmail(dto.Email)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new CreateUserCommand(dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في إنشاء المستخدم", result.Message);
    }
} 