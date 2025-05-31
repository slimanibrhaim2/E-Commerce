using MediatR;
using Core.Result;
using Users.Application.DTOs;
using Core.Pagination;

namespace Users.Application.Queries.GetUsersByName;

public record GetUsersByNameQuery(string Name, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<UserDTO>>>; 