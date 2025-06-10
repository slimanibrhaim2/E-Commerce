using MediatR;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Catalogs.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Catalogs.Application.Queries.GetServicesByIds
{
    public class GetServicesByIdsQueryHandler : IRequestHandler<GetServicesByIdsQuery, IEnumerable<ServiceDetailsDTO>>
    {
        private readonly IServiceRepository _serviceRepo;
        public GetServicesByIdsQueryHandler(IServiceRepository serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task<IEnumerable<ServiceDetailsDTO>> Handle(GetServicesByIdsQuery request, CancellationToken cancellationToken)
        {
            var services = await _serviceRepo.GetByIdsAsync(request.ServiceIds);
            return services.Select(s => new ServiceDetailsDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                UserId = s.UserId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Media = s.Media?.Select(m => new MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    ItemId = m.BaseItemId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDTO>()
            });
        }
    }
} 