using Xunit;
using Moq;
using Users.Application.Commands.DeleteAddress;
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

public class DeleteAddressCommandHandlerTests
{
    private readonly Mock<IAddressRepository> _addressRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<DeleteAddressCommandHandler>> _loggerMock = new();

    private DeleteAddressCommandHandler CreateHandler() =>
        new(_addressRepoMock.Object, _uowMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "نجاح الحذف عند وجود العنوان وعدم حذفه مسبقًا")]
    public async Task DeleteAddress_Success_WhenAddressExistsAndNotDeleted()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var addressEntity = new Address { Id = addressId, DeletedAt = null };
        _addressRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Address, bool>>>())).ReturnsAsync(new List<Address> { addressEntity });
        var handler = CreateHandler();
        var cmd = new DeleteAddressCommand(addressId);
        _uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم حذف العنوان بنجاح", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند عدم وجود العنوان")]
    public async Task DeleteAddress_Fails_WhenAddressNotFound()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _addressRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Address, bool>>>())).ReturnsAsync(new List<Address>());
        var handler = CreateHandler();
        var cmd = new DeleteAddressCommand(addressId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("العنوان غير موجود", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حذف العنوان مسبقًا")]
    public async Task DeleteAddress_Fails_WhenAlreadyDeleted()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var addressEntity = new Address { Id = addressId, DeletedAt = DateTime.UtcNow };
        _addressRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Address, bool>>>())).ReturnsAsync(new List<Address> { addressEntity });
        var handler = CreateHandler();
        var cmd = new DeleteAddressCommand(addressId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("العنوان محذوف بالفعل", result.Message);
    }

    [Fact(DisplayName = "فشل الحذف عند حدوث استثناء داخلي")]
    public async Task DeleteAddress_Fails_WhenExceptionThrown()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _addressRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Address, bool>>>())).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new DeleteAddressCommand(addressId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فشل في حذف العنوان", result.Message);
    }
} 