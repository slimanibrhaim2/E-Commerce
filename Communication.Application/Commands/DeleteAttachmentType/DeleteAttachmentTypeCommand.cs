using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteAttachmentType;

public record DeleteAttachmentTypeCommand(Guid Id) : IRequest<Result<bool>>; 