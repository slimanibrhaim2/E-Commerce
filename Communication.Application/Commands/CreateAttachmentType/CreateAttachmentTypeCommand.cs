using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateAttachmentType;

public record CreateAttachmentTypeCommand(CreateAttachmentTypeDTO AttachmentType) : IRequest<Result<Guid>>; 