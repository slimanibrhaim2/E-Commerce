using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetAttachmentTypeById;

public record GetAttachmentTypeByIdQuery(Guid Id) : IRequest<Result<AttachmentTypeDTO>>; 