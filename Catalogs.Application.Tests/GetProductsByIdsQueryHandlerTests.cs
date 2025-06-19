//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Catalogs.Application.Queries.GetProductsByIds;
//using Catalogs.Domain.Entities;
//using Catalogs.Domain.Repositories;
//using Moq;
//using Shared.Contracts.DTOs;
//using Shared.Contracts.Queries;
//using Xunit;

//namespace Catalogs.Application.Tests
//{
//    public class GetProductsByIdsQueryHandlerTests
//    {
//        private readonly Mock<IProductRepository> _productRepoMock;
//        private readonly Mock<IFeatureRepository> _featureRepoMock;
//        private readonly GetProductsByIdsQueryHandler _handler;

//        public GetProductsByIdsQueryHandlerTests()
//        {
//            _productRepoMock = new Mock<IProductRepository>();
//            _featureRepoMock = new Mock<IFeatureRepository>();
//            _handler = new GetProductsByIdsQueryHandler(_productRepoMock.Object, _featureRepoMock.Object);
//        }

//        [Fact]
//        public async Task Handle_ReturnsProductDetailsDTOs_WithFeatures()
//        {
//            // Arrange
//            var productId = Guid.NewGuid();
//            var products = new List<Product>
//            {
//                new Product
//                {
//                    Id = productId,
//                    Name = "Test Product",
//                    Description = "Desc",
//                    Price = 100,
//                    CategoryId = Guid.NewGuid(),
//                    SKU = "SKU1",
//                    StockQuantity = 10,
//                    IsAvailable = true,
//                    UserId = Guid.NewGuid(),
//                    CreatedAt = DateTime.UtcNow,
//                    UpdatedAt = DateTime.UtcNow,
//                    Media = new List<Media>
//                    {
//                        new Media
//                        {
//                            Id = Guid.NewGuid(),
//                            MediaUrl = "url",
//                            MediaTypeId = Guid.NewGuid(),
//                            BaseItemId = Guid.NewGuid(),
//                            CreatedAt = DateTime.UtcNow,
//                            UpdatedAt = DateTime.UtcNow
//                        }
//                    }
//                }
//            };
//            var features = new List<ProductFeature>
//            {
//                new ProductFeature
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Color",
//                    Value = "Red",
//                    BaseItemId = productId,
//                    CreatedAt = DateTime.UtcNow,
//                    UpdatedAt = DateTime.UtcNow
//                }
//            };
//            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//                .ReturnsAsync(products);
//            _featureRepoMock.Setup(r => r.GetProductFeaturesByEntityIdAsync(productId))
//                .ReturnsAsync(features);

//            var query = new GetProductsByIdsQuery(new List<Guid> { productId });

//            // Act
//            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

//            // Assert
//            Assert.Single(result);
//            Assert.Equal("Test Product", result[0].Name);
//            Assert.Single(result[0].Features);
//            Assert.Equal("Color", result[0].Features[0].Name);
//        }

//        [Fact]
//        public async Task Handle_ReturnsEmptyList_WhenNoProductsFound()
//        {
//            // Arrange
//            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//                .ReturnsAsync(new List<Product>());
//            var query = new GetProductsByIdsQuery(new List<Guid> { Guid.NewGuid() });

//            // Act
//            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

//            // Assert
//            Assert.Empty(result);
//        }

//        [Fact]
//        public async Task Handle_ProductWithoutFeatures_ReturnsEmptyFeaturesList()
//        {
//            // Arrange
//            var productId = Guid.NewGuid();
//            var products = new List<Product>
//            {
//                new Product
//                {
//                    Id = productId,
//                    Name = "No Feature Product",
//                    Description = "Desc",
//                    Price = 50,
//                    CategoryId = Guid.NewGuid(),
//                    SKU = "SKU2",
//                    StockQuantity = 5,
//                    IsAvailable = true,
//                    UserId = Guid.NewGuid(),
//                    CreatedAt = DateTime.UtcNow,
//                    UpdatedAt = DateTime.UtcNow,
//                    Media = new List<Media>()
//                }
//            };
//            _productRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
//                .ReturnsAsync(products);
//            _featureRepoMock.Setup(r => r.GetProductFeaturesByEntityIdAsync(productId))
//                .ReturnsAsync(new List<ProductFeature>());

//            var query = new GetProductsByIdsQuery(new List<Guid> { productId });

//            // Act
//            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

//            // Assert
//            Assert.Single(result);
//            Assert.Empty(result[0].Features);
//        }
//    }
//} 