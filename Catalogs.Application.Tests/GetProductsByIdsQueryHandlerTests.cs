using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalogs.Application.Queries.GetProductsByIds;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Moq;
using Shared.Contracts.DTOs;
using Shared.Contracts.Queries;
using Xunit;
using Microsoft.Extensions.Logging;
using Core.Result;
using Core.Pagination;

namespace Catalogs.Application.Tests
{
    public class GetProductsByIdsQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ILogger<GetProductsByIdsQueryHandler>> _loggerMock;
        private readonly GetProductsByIdsQueryHandler _handler;

        public GetProductsByIdsQueryHandlerTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<GetProductsByIdsQueryHandler>>();
            _handler = new GetProductsByIdsQueryHandler(_productRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsProductDetailsDTOs_WithFeatures()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product
                {
                    Id = productId,
                    Name = "Test Product",
                    Description = "Desc",
                    Price = 100,
                    CategoryId = Guid.NewGuid(),
                    SKU = "SKU1",
                    SerialNumber = "SN001",
                    StockQuantity = 10,
                    IsAvailable = true,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Media = new List<Media>
                    {
                        new Media
                        {
                            Id = Guid.NewGuid(),
                            MediaUrl = "url",
                            MediaTypeId = Guid.NewGuid(),
                            BaseItemId = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    },
                    Features = new List<ProductFeature>
                    {
                        new ProductFeature
                        {
                            Id = Guid.NewGuid(),
                            Name = "Color",
                            Value = "Red",
                            BaseItemId = productId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    }
                }
            };
            
            var paginatedProducts = PaginatedResult<Product>.Create(products, 1, 10, 1);
            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedProducts);

            var query = new GetProductsByIdsQuery(new List<Guid> { productId }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Single(result.Data.Data);
            Assert.Equal("Test Product", result.Data.Data.First().Name);
            Assert.Single(result.Data.Data.First().Features);
            Assert.Equal("Color", result.Data.Data.First().Features.First().Name);
            Assert.Single(result.Data.Data.First().Media);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoProductsFound()
        {
            // Arrange
            var emptyPaginatedProducts = PaginatedResult<Product>.Create(new List<Product>(), 1, 10, 0);
            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(emptyPaginatedProducts);
            var query = new GetProductsByIdsQuery(new List<Guid> { Guid.NewGuid() }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Empty(result.Data.Data);
            Assert.Contains("No products found for the provided IDs", result.Message);
        }

        [Fact]
        public async Task Handle_ProductWithoutFeatures_ReturnsEmptyFeaturesList()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product
                {
                    Id = productId,
                    Name = "No Feature Product",
                    Description = "Desc",
                    Price = 50,
                    CategoryId = Guid.NewGuid(),
                    SKU = "SKU2",
                    SerialNumber = "SN002",
                    StockQuantity = 5,
                    IsAvailable = true,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Media = new List<Media>(),
                    Features = new List<ProductFeature>()
                }
            };
            
            var paginatedProducts = PaginatedResult<Product>.Create(products, 1, 10, 1);
            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedProducts);

            var query = new GetProductsByIdsQuery(new List<Guid> { productId }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Single(result.Data.Data);
            Assert.Empty(result.Data.Data.First().Features);
            Assert.Empty(result.Data.Data.First().Media);
        }

        [Fact]
        public async Task Handle_Fails_WhenProductIdsListIsEmpty()
        {
            // Arrange
            var query = new GetProductsByIdsQuery(new List<Guid>(), new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.ValidationError, result.ResultStatus);
            Assert.Equal("Product IDs list cannot be empty", result.Message);
        }

        [Fact]
        public async Task Handle_Fails_WhenProductIdsContainEmptyGuid()
        {
            // Arrange
            var query = new GetProductsByIdsQuery(new List<Guid> { Guid.NewGuid(), Guid.Empty }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.ValidationError, result.ResultStatus);
            Assert.Equal("All product IDs must be valid GUIDs", result.Message);
        }



        [Fact]
        public async Task Handle_Fails_WhenExceptionThrown()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));
            var query = new GetProductsByIdsQuery(new List<Guid> { Guid.NewGuid() }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.Failed, result.ResultStatus);
            Assert.Equal("An unexpected error occurred while retrieving products. Please try again later.", result.Message);
        }
    }
} 