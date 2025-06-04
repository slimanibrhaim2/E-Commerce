using Xunit;
using Moq;
using Users.Application.Commands.AddAddressByUserId;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class AddAddressByUserIdCommandHandlerTests
{
    private readonly Mock<IAddressRepository> _addressRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<AddAddressByUserIdCommandHandler>> _loggerMock = new();

    private AddAddressByUserIdCommandHandler CreateHandler() =>
        new(_addressRepoMock.Object, _userRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح الإضافة عند صحة البيانات وعدم وجود العنوان مسبقًا")]
    public async Task AddAddress_Success_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 31.0 };
        var userEntity = new User { Id = userId };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userEntity);
        _addressRepoMock.Setup(r => r.GetAddressesByUserId(userId)).ReturnsAsync(new List<Address>());
        _addressRepoMock.Setup(r => r.AddAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم إضافة العنوان بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند معرف مستخدم غير صالح")]
    public async Task AddAddress_Fails_WhenUserIdInvalid()
    {
        // Arrange
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 31.0 };
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(Guid.Empty, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("معرف المستخدم غير صالح", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند اسم العنوان مفقود")]
    public async Task AddAddress_Fails_WhenNameMissing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = null, Latitude = 30.0, Longitude = 31.0 };
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("اسم العنوان مطلوب", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند خط العرض غير صالح")]
    public async Task AddAddress_Fails_WhenLatitudeInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 100.0, Longitude = 31.0 };
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("قيمة خط العرض غير صالحة", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند خط الطول غير صالح")]
    public async Task AddAddress_Fails_WhenLongitudeInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 200.0 };
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("قيمة خط الطول غير صالحة", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند عدم وجود المستخدم")]
    public async Task AddAddress_Fails_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 31.0 };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("المستخدم غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند وجود العنوان مسبقًا")]
    public async Task AddAddress_Fails_WhenAddressAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 31.0 };
        var addressEntity = new Address { Name = "المنزل", Latitude = 30.0, Longitude = 31.0, DeletedAt = null };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _addressRepoMock.Setup(r => r.GetAddressesByUserId(userId)).ReturnsAsync(new List<Address> { addressEntity });
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("هذا العنوان موجود بالفعل", result.Message);
    }

    [Fact(DisplayName = "فشل الإضافة عند حدوث استثناء داخلي")]
    public async Task AddAddress_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AddAddressDTO { Name = "المنزل", Latitude = 30.0, Longitude = 31.0 };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new AddAddressByUserIdCommand(userId, dto);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("حدث خطأ أثناء إضافة العنوان", result.Message);
    }
} 