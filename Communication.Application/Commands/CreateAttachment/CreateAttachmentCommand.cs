using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateAttachment;

public record CreateAttachmentCommand(CreateAttachmentDTO Attachment) : IRequest<Result<Guid>>; 