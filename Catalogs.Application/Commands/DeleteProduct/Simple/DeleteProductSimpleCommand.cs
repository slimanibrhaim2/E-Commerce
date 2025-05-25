using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteProduct.Simple;

public record DeleteProductSimpleCommand(Guid Id) : IRequest<Result<bool>>;