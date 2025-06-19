using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateProduct.Simple;

public record UpdateProductSimpleCommand(Guid Id, CreateProductDTO ProductDto, Guid UserId) : IRequest<Result<bool>>;