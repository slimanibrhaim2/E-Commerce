using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetServiceById;

public record GetServiceByIdQuery(Guid Id, Guid UserId) : IRequest<Result<ServiceDTO>>; 