using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteProduct.Aggregate;

public record DeleteProductAggregateCommand(Guid Id) : IRequest<Result<bool>>;