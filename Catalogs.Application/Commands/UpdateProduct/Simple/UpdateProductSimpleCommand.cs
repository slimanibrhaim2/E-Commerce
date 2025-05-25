using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateProduct.Simple;

public record UpdateProductSimpleCommand(Guid Id, ProductDTO ProductDto) : IRequest<Result<bool>>;