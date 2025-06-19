using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateProduct.Aggregate;

public record UpdateProductAggregateCommand(Guid Id, CreateProductAggregateDTO Product, Guid UserId) : IRequest<Result<bool>>;