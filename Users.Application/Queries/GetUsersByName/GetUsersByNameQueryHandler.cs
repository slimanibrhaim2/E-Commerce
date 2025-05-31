using MediatR;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Users.Application.Queries.GetUsersByName;

public class GetUsersByNameQueryHandler : IRequestHandler<GetUsersByNameQuery, Result<PaginatedResult<UserDTO>>>
{
    private readonly IUserRepository _repository;

    public GetUsersByNameQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<UserDTO>>> Handle(GetUsersByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = (await _repository.GetUsersByNameAsync(request.Name)).ToList();
            var totalCount = users.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var dtos = paged.Select(u => new UserDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                ProfilePhoto = u.ProfilePhoto,
                Description = u.Description
            }).ToList();
            var paginated = PaginatedResult<UserDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<UserDTO>>.Ok(
                data: paginated,
                message: "Users retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<UserDTO>>.Fail(
                message: $"Failed to get users: {ex.Message}",
                errorType: "GetUsersByNameFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 