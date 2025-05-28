using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetAttachmentById;

public record GetAttachmentByIdQuery(Guid Id) : IRequest<Result<AttachmentDTO>>; 