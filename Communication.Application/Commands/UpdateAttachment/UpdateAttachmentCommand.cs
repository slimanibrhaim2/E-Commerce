using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateAttachment;

public record UpdateAttachmentCommand(Guid Id, CreateAttachmentDTO Attachment) : IRequest<Result<bool>>; 