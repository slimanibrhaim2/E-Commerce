using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetProductById;
 
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDTO>>; 