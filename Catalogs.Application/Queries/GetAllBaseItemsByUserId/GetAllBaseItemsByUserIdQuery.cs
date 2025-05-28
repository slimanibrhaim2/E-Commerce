using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using System.Collections.Generic;

namespace Catalogs.Application.Queries.GetAllBaseItemsByUserId;

public record GetAllBaseItemsByUserIdQuery(Guid UserId) : IRequest<Result<List<BaseItemDTO>>>; 