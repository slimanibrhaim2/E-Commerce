using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllBaseContents;

public record GetAllBaseContentsQuery() : IRequest<Result<List<BaseContentDTO>>>; 