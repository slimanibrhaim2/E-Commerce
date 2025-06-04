using Xunit;
using Moq;
using Users.Application.Queries.GetAddressesByUserId;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Pagination;
using Core.Result;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class GetAddressesByUserIdQueryHandlerTests
{
    private readonly Mock<IAddressRepository> _addressRepoMock = new();
    private GetAddressesByUserIdQueryHandler CreateHandler() => new(_addressRepoMock.Object);

    [Fact(DisplayName = "نجاح جلب العناوين عند وجود بيانات")]
    public async Task GetAddressesByUserId_Success_WhenAddressesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var addresses = new List<Address> { new Address { Id = Guid.NewGuid(), Name = "المنزل", Latitude = 30.0, Longitude = 31.0 } };
        _addressRepoMock.Setup(r => r.GetAddressesByUserId(userId)).ReturnsAsync(addresses);
        var handler = CreateHandler();
        var cmd = new GetAddressesByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب العناوين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Data.Any());
    }

    [Fact(DisplayName = "نجاح جلب العناوين عند عدم وجود بيانات (قائمة فارغة)")]
    public async Task GetAddressesByUserId_Success_WhenNoAddressesExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _addressRepoMock.Setup(r => r.GetAddressesByUserId(userId)).ReturnsAsync(new List<Address>());
        var handler = CreateHandler();
        var cmd = new GetAddressesByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("تم جلب العناوين بنجاح", result.Message);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Data);
    }

    [Fact(DisplayName = "فشل الجلب عند حدوث استثناء داخلي")]
    public async Task GetAddressesByUserId_Fails_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _addressRepoMock.Setup(r => r.GetAddressesByUserId(userId)).ThrowsAsync(new Exception("DB error"));
        var handler = CreateHandler();
        var cmd = new GetAddressesByUserIdQuery(userId, new Core.Pagination.PaginationParameters());

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("حدث خطأ أثناء جلب العناوين", result.Message);
    }
} 