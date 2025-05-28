using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetBaseContentById;

public record GetBaseContentByIdQuery(Guid Id) : IRequest<Result<BaseContentDTO>>; 