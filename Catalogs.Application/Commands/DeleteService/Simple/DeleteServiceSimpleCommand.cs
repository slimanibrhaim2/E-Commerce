using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteService.Simple;

public record DeleteServiceSimpleCommand(Guid Id) : IRequest<Result<bool>>;