using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IRequest<Result<BrandDTO>>; 