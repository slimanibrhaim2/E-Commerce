using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateProduct.Simple;

public record CreateProductCommand(CreateProductDTO ProductDto, Guid UserId) : IRequest<Result<Guid>>; 