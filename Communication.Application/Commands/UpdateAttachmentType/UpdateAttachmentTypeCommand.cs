using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateAttachmentType;

public record UpdateAttachmentTypeCommand(Guid Id, CreateAttachmentTypeDTO AttachmentType) : IRequest<Result<bool>>; 