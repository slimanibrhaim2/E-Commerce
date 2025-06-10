using MediatR;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Catalogs.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Catalogs.Application.Queries.GetProductsByIds
{
    public class GetProductsByIdsQueryHandler : IRequestHandler<GetProductsByIdsQuery, IEnumerable<ProductDetailsDTO>>
    {
        private readonly IProductRepository _productRepo;
        private readonly IFeatureRepository _featureRepo;
        public GetProductsByIdsQueryHandler(IProductRepository productRepo, IFeatureRepository featureRepo)
        {
            _productRepo = productRepo;
            _featureRepo = featureRepo;
        }

        public async Task<IEnumerable<ProductDetailsDTO>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetByIdsAsync(request.ProductIds);
            var productList = products.ToList();
            var result = new List<ProductDetailsDTO>();
            foreach (var p in productList)
            {
                var features = await _featureRepo.GetProductFeaturesByEntityIdAsync(p.Id);
                result.Add(new ProductDetailsDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    SKU = p.SKU,
                    StockQuantity = p.StockQuantity,
                    IsAvailable = p.IsAvailable,
                    UserId = p.UserId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Media = p.Media?.Select(m => new MediaDTO
                    {
                        Id = m.Id,
                        Url = m.MediaUrl,
                        MediaTypeId = m.MediaTypeId,
                        ItemId = m.BaseItemId,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    }).ToList() ?? new List<MediaDTO>(),
                    Features = features.Select(f => new ProductFeatureDTO
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Value = f.Value,
                        ProductId = f.BaseItemId,
                        CreatedAt = f.CreatedAt,
                        UpdatedAt = f.UpdatedAt
                    }).ToList()
                });
            }
            return result;
        }
    }
} 