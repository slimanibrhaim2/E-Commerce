using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, CreatCategoryDTO Category) : IRequest<Result<bool>>; 