using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDTO Category) : IRequest<Result<Guid>>; 