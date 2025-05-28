using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid Id) : IRequest<Result<bool>>; 