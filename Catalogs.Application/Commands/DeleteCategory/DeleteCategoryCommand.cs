using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result<bool>>; 