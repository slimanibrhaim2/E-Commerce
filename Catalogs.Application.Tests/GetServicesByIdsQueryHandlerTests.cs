using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalogs.Application.Queries.GetServicesByIds;
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
    public class GetServicesByIdsQueryHandlerTests
    {
        private readonly Mock<IServiceRepository> _serviceRepoMock;
        private readonly Mock<ILogger<GetServicesByIdsQueryHandler>> _loggerMock;
        private readonly GetServicesByIdsQueryHandler _handler;

        public GetServicesByIdsQueryHandlerTests()
        {
            _serviceRepoMock = new Mock<IServiceRepository>();
            _loggerMock = new Mock<ILogger<GetServicesByIdsQueryHandler>>();
            _handler = new GetServicesByIdsQueryHandler(_serviceRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsServiceDetailsDTOs_WithMedia()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var services = new List<Service>
            {
                new Service
                {
                    Id = serviceId,
                    Name = "Test Service",
                    Description = "Desc",
                    Price = 200,
                    CategoryId = Guid.NewGuid(),
                    ServiceType = "TypeA",
                    Duration = 60,
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
                    }
                }
            };
            
            var paginatedServices = PaginatedResult<Service>.Create(services, 1, 10, 1);
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedServices);

            var query = new GetServicesByIdsQuery(new List<Guid> { serviceId }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Single(result.Data.Data);
            Assert.Equal("Test Service", result.Data.Data.First().Name);
            Assert.Single(result.Data.Data.First().Media);
            Assert.Equal("url", result.Data.Data.First().Media.First().Url);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoServicesFound()
        {
            // Arrange
            var emptyPaginatedServices = PaginatedResult<Service>.Create(new List<Service>(), 1, 10, 0);
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(emptyPaginatedServices);
            var query = new GetServicesByIdsQuery(new List<Guid> { Guid.NewGuid() }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Empty(result.Data.Data);
            Assert.Contains("No services found for the provided IDs", result.Message);
        }

        [Fact]
        public async Task Handle_ServiceWithoutMedia_ReturnsEmptyMediaList()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var services = new List<Service>
            {
                new Service
                {
                    Id = serviceId,
                    Name = "No Media Service",
                    Description = "Desc",
                    Price = 80,
                    CategoryId = Guid.NewGuid(),
                    ServiceType = "TypeB",
                    Duration = 30,
                    IsAvailable = true,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Media = new List<Media>()
                }
            };
            
            var paginatedServices = PaginatedResult<Service>.Create(services, 1, 10, 1);
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedServices);

            var query = new GetServicesByIdsQuery(new List<Guid> { serviceId }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ResultStatus.Success, result.ResultStatus);
            Assert.Single(result.Data.Data);
            Assert.Empty(result.Data.Data.First().Media);
        }

        [Fact]
        public async Task Handle_Fails_WhenServiceIdsListIsEmpty()
        {
            // Arrange
            var query = new GetServicesByIdsQuery(new List<Guid>(), new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.ValidationError, result.ResultStatus);
            Assert.Equal("Service IDs list cannot be empty", result.Message);
        }

        [Fact]
        public async Task Handle_Fails_WhenServiceIdsContainEmptyGuid()
        {
            // Arrange
            var query = new GetServicesByIdsQuery(new List<Guid> { Guid.NewGuid(), Guid.Empty }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.ValidationError, result.ResultStatus);
            Assert.Equal("All service IDs must be valid GUIDs", result.Message);
        }



        [Fact]
        public async Task Handle_Fails_WhenExceptionThrown()
        {
            // Arrange
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));
            var query = new GetServicesByIdsQuery(new List<Guid> { Guid.NewGuid() }, new PaginationParameters { PageNumber = 1, PageSize = 10 });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ResultStatus.Failed, result.ResultStatus);
            Assert.Equal("An unexpected error occurred while retrieving services. Please try again later.", result.Message);
        }
    }
} 