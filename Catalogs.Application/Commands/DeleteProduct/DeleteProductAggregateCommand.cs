using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteProduct;

public record DeleteProductAggregateCommand(Guid Id) : IRequest<Result<bool>>;