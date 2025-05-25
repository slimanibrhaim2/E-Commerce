using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateProduct.Aggregate;

public record CreateProductAggregateCommand(CreateProductAggregateDTO Product) : IRequest<Result<Guid>>; 