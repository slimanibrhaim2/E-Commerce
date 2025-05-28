using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllAttachments;

public record GetAllAttachmentsQuery() : IRequest<Result<List<AttachmentDTO>>>; 