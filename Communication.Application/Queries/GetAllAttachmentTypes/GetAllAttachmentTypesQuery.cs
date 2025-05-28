using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllAttachmentTypes;

public record GetAllAttachmentTypesQuery() : IRequest<Result<List<AttachmentTypeDTO>>>; 