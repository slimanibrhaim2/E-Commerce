using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteMediaType;
 
public record DeleteMediaTypeCommand(Guid Id) : IRequest<Result<bool>>; 