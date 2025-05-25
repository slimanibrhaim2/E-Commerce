using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteMedia
{
    public record DeleteMediaCommand(Guid Id) : IRequest<Result<bool>>;
} 