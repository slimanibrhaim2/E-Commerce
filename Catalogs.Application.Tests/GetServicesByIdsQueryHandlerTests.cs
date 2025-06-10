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

namespace Catalogs.Application.Tests
{
    public class GetServicesByIdsQueryHandlerTests
    {
        private readonly Mock<IServiceRepository> _serviceRepoMock;
        private readonly GetServicesByIdsQueryHandler _handler;

        public GetServicesByIdsQueryHandlerTests()
        {
            _serviceRepoMock = new Mock<IServiceRepository>();
            _handler = new GetServicesByIdsQueryHandler(_serviceRepoMock.Object);
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
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(services);

            var query = new GetServicesByIdsQuery(new List<Guid> { serviceId });

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Service", result[0].Name);
            Assert.Single(result[0].Media);
            Assert.Equal("url", result[0].Media[0].Url);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoServicesFound()
        {
            // Arrange
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Service>());
            var query = new GetServicesByIdsQuery(new List<Guid> { Guid.NewGuid() });

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.Empty(result);
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
            _serviceRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(services);

            var query = new GetServicesByIdsQuery(new List<Guid> { serviceId });

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Empty(result[0].Media);
        }
    }
} 